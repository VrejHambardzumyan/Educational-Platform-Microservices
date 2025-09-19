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
        public async Task<IActionResult> SignUp(SignUpRequestDTO request)
        {
            await _authService.RegisterUser(request.UserName, request.Password);
            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(SignInRequestDTO request)
        {
           var tokenResponse =  await _authService.LoginUser(request.UserName, request.Password);
            if (tokenResponse != null)
            {
                return Ok(tokenResponse);
            }
            return Unauthorized(new { message = "Invalid login or password" });
        }
    }
}
