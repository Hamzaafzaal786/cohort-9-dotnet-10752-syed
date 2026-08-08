using Microsoft.AspNetCore.Mvc;
using Serilog;
using TaskManagementSystem.Application.DTOs.Auth;
using TaskManagementSystem.Application.Interfaces;

namespace TaskManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                Log.Information("User {Email} logged in successfully", loginDto.Email);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Warning("Login failed for {Email}: {Message}", loginDto.Email, ex.Message);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during login for {Email}", loginDto.Email);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                Log.Information("User {Email} registered successfully", registerDto.Email);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                Log.Warning("Registration failed for {Email}: {Message}", registerDto.Email, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during registration for {Email}", registerDto.Email);
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error refreshing token");
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (userId != null)
                {
                    await _authService.LogoutAsync(userId);
                    Log.Information("User {UserId} logged out", userId);
                }
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during logout");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }
    }
}