using API.Middleware;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Register(
            [FromBody] RegisterDto registerDto,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.RegisterAsync(registerDto, cancellationToken);
                return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(CreateErrorResponse(400, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateErrorResponse(400, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(CreateErrorResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return StatusCode(500, CreateErrorResponse(500, "An unexpected error occurred during registration."));
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login(
            [FromBody] LoginDto loginDto,
            CancellationToken cancellationToken)
        {
            try
            {
                var authResponse = await _authService.LoginAsync(loginDto, cancellationToken);
                return Ok(authResponse);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(CreateErrorResponse(400, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateErrorResponse(400, ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(CreateErrorResponse(401, "Invalid email or password."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, CreateErrorResponse(500, "An unexpected error occurred during login."));
            }
        }

        private ErrorResponse CreateErrorResponse(int statusCode, string message)
        {
            return new ErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                Path = HttpContext.Request.Path,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}