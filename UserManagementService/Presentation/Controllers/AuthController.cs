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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Registration failed.", detail = ex.Message });
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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Login failed.", detail = ex.Message });
            }

            //[HttpPost("refresh")]   
            //public async Task<IActionResult> Refresh(RefreshTokenRequestDTO request)
            //{
            //    var tokenResponse = await _authService.RefreshToken(request.RefreshToken);
            //    if (tokenResponse == null)
            //        return Unauthorized(new { message = "Invalid refresh token" });

            //    return Ok(tokenResponse);
            //}

        }
    }
}
