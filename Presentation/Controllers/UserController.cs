using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models;
using UserManagementService.Presentation;

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

        [HttpPost(Name = "AddUser")]
        public async Task<IActionResult> AddUser(UserModel user)
        {
            await _userService.AddUser(user);
            return Ok();
        }
    }
}
