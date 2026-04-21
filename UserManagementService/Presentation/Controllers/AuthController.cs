using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.DTOs;

namespace UserManagementService.Presentation.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp(SignUpRequestDto request)
        {
            try
            {
                var tokenResponse = await _authService.RegisterUserAsync(request.UserName, request.Password, request.Email);
                return Ok(tokenResponse);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Registration failed." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(SignInRequestDto request)
        {
            try
            {
                var tokenResponse = await _authService.LoginUserAsync(request.UserName, request.Password);
                return Ok(tokenResponse);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Login failed." });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequestDto request)
        {
            try
            {
                var tokenResponse = await _authService.RefreshTokenAsync(request.RefreshToken);
                return Ok(tokenResponse);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Token refresh failed." });
            }
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke(RefreshTokenRequestDto request)
        {
            await _authService.RevokeTokenAsync(request.RefreshToken);
            return NoContent();
        }
    }
}
