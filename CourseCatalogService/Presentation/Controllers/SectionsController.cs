using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Application.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseCatalogService.Presentation.Controllers
{
    [ApiController]
    [Route("api/courses/{courseId}/sections")]
    public class SectionsController(ISectionService sectionService, ILogger<SectionsController> logger) : ControllerBase
    {
        private readonly ISectionService _sectionService = sectionService;
        private readonly ILogger<SectionsController> _logger = logger;

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Create(int courseId, [FromBody] SectionRequestDto dto, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var result = await _sectionService.CreateAsync(courseId, dto, userId.Value, User.IsInRole("Admin"), cancellationToken);
                return CreatedAtAction(nameof(GetByCourse), new { courseId }, result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create section");
                return StatusCode(500, new { message = "Failed to create section." });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCourse(int courseId, CancellationToken cancellationToken)
        {
            var result = await _sectionService.GetByCourseIdAsync(courseId, cancellationToken);
            return Ok(result);
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
