using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITokenService tokenService,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
        {
            if (registerDto == null)
                throw new ArgumentNullException(nameof(registerDto));

            _logger.LogInformation("Registering user: {Username}", registerDto.Username);

            ValidateRegistrationInput(registerDto);

            // Check username uniqueness
            var existingUsername = await _unitOfWork.Users.FindAsync(
                u => u.Username.ToLower() == registerDto.Username.ToLower(),
                cancellationToken);

            if (existingUsername.Any())
                throw new InvalidOperationException($"Username '{registerDto.Username}' is already taken.");

            // Check email uniqueness
            var existingEmail = await _unitOfWork.Users.FindAsync(
                u => u.Email.ToLower() == registerDto.Email.ToLower(),
                cancellationToken);

            if (existingEmail.Any())
                throw new InvalidOperationException($"Email '{registerDto.Email}' is already registered.");

            var user = _mapper.Map<User>(registerDto);
            user.CreatedAt = DateTime.UtcNow;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password, 11);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                await _unitOfWork.Users.AddAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("User registered successfully: {Username}", user.Username);
                return _mapper.Map<UserDto>(user);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            if (loginDto == null)
                throw new ArgumentNullException(nameof(loginDto));

            _logger.LogInformation("Login attempt for: {Email}", loginDto.Email);

            ValidateLoginInput(loginDto);

            var users = await _unitOfWork.Users.FindAsync(
                u => u.Email.ToLower() == loginDto.Email.ToLower(),
                cancellationToken);

            var user = users.FirstOrDefault();

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var userDto = _mapper.Map<UserDto>(user);
            var token = _tokenService.GenerateToken(userDto);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };
        }

        public async Task<bool> ValidateUserAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return false;

            var users = await _unitOfWork.Users.FindAsync(
                u => u.Email.ToLower() == email.ToLower(),
                cancellationToken);

            var user = users.FirstOrDefault();

            return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var users = await _unitOfWork.Users.FindAsync(
                u => u.Email.ToLower() == email.ToLower(),
                cancellationToken);

            var user = users.FirstOrDefault();

            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var users = await _unitOfWork.Users.FindAsync(
                u => u.Username.ToLower() == username.ToLower(),
                cancellationToken);

            var user = users.FirstOrDefault();

            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<bool> ChangePasswordAsync(
            int userId,
            string currentPassword,
            string newPassword,
            CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            ValidatePassword(newPassword);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, 11);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        private void ValidateRegistrationInput(RegisterDto registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.Username))
                throw new ArgumentException("Username is required.", nameof(registerDto.Username));

            if (string.IsNullOrWhiteSpace(registerDto.Email))
                throw new ArgumentException("Email is required.", nameof(registerDto.Email));

            if (!IsValidEmail(registerDto.Email))
                throw new ArgumentException("Invalid email format.", nameof(registerDto.Email));

            if (string.IsNullOrWhiteSpace(registerDto.FirstName))
                throw new ArgumentException("First name is required.", nameof(registerDto.FirstName));

            if (string.IsNullOrWhiteSpace(registerDto.LastName))
                throw new ArgumentException("Last name is required.", nameof(registerDto.LastName));

            ValidatePassword(registerDto.Password);
        }

        private void ValidateLoginInput(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email))
                throw new ArgumentException("Email is required.", nameof(loginDto.Email));

            if (string.IsNullOrWhiteSpace(loginDto.Password))
                throw new ArgumentException("Password is required.", nameof(loginDto.Password));
        }

        private void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.");

            if (password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long.");

            if (!password.Any(char.IsUpper))
                throw new ArgumentException("Password must contain at least one uppercase letter.");

            if (!password.Any(char.IsLower))
                throw new ArgumentException("Password must contain at least one lowercase letter.");

            if (!password.Any(char.IsDigit))
                throw new ArgumentException("Password must contain at least one digit.");

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                throw new ArgumentException("Password must contain at least one special character.");
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}