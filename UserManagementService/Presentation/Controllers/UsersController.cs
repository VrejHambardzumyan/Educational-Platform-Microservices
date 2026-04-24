using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagementService.Application.Authorization;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.DTOs;

namespace UserManagementService.Presentation.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>Returns the authenticated user's own profile.</summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var profile = await _userService.GetByIdAsync(userId.Value);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>Updates the authenticated user's own profile.</summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(UpdateProfileRequestDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var updated = await _userService.UpdateProfileAsync(userId.Value, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        /// <summary>Enables or disables OTP for the authenticated user.</summary>
        [HttpPut("me/otp")]
        public async Task<IActionResult> UpdateOtpPreference([FromBody] UpdateOtpPreferenceRequestDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var result = await _userService.SetOtpEnabledAsync(userId.Value, dto.Enabled);
            if (!result) return NotFound();
            return Ok(new { otpEnabled = dto.Enabled });
        }

        /// <summary>Returns any user's profile. Requires the <c>users.read_any</c> permission.</summary>
        [HttpGet("{id}")]
        [HasPermission(Permissions.UsersReadAny)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var profile = await _userService.GetByIdAsync(id);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>Returns a paginated list of all users. Requires the <c>users.manage</c> permission.</summary>
        [HttpGet]
        [HasPermission(Permissions.UsersManage)]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var result = await _userService.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Assigns a role to the specified user.
        /// Authenticated users may switch their own role between Student and Instructor.
        /// Changing another user's role, or assigning Admin, requires the <c>users.manage</c> permission.
        /// </summary>
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, UpdateRoleRequestDto dto)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var isAdmin = User.HasClaim("permissions", Permissions.UsersManage);

            if (id == currentUserId.Value && !isAdmin)
            {
                try
                {
                    var result = await _userService.SwitchOwnRoleAsync(id, dto.Role);
                    if (!result) return NotFound();
                    return NoContent();
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            if (!isAdmin) return Forbid();

            try
            {
                var result = await _userService.UpdateRoleAsync(id, dto.Role);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Soft-deletes the specified user. Requires the <c>users.manage</c> permission.</summary>
        [HttpDelete("{id}")]
        [HasPermission(Permissions.UsersManage)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.SoftDeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst("userId");
            if (claim != null && int.TryParse(claim.Value, out var id))
                return id;
            return null;
        }
    }
}
