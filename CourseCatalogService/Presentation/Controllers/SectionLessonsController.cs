using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Application.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseCatalogService.Presentation.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Instructor")]
    [Route("api/sections/{sectionId}/lessons")]
    public class SectionLessonsController(ISectionService sectionService, ILogger<SectionLessonsController> logger) : ControllerBase
    {
        private readonly ISectionService _sectionService = sectionService;
        private readonly ILogger<SectionLessonsController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> Add(int sectionId, [FromBody] LessonRequestDto dto, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var result = await _sectionService.AddLessonAsync(sectionId, dto, userId.Value, User.IsInRole("Admin"), cancellationToken);
                return CreatedAtAction(nameof(Get), new { sectionId }, result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add lesson to section");
                return StatusCode(500, new { message = "Failed to add lesson." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int sectionId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _sectionService.GetLessonsAsync(sectionId, cancellationToken);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
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
