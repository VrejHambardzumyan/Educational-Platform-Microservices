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
            var tokenResponse  = await _authService.RegisterUser(request.UserName, request.Password);
            return Ok(tokenResponse);
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
