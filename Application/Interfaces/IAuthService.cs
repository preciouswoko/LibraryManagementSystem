using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
        Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<UserDto?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
        Task<UserDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default);
        Task<bool> ValidateUserAsync(string email, string password, CancellationToken cancellationToken = default);
    }
}
