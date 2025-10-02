using Microsoft.AspNetCore.Mvc;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.DTOs;

namespace UserManagementService.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser(UserDto user)
        {
            await _userService.AddUserAsync(user);
            return Ok();
        }
    }
}
