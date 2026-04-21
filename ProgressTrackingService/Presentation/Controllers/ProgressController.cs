using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Application.Interfaces;
using ProgressTrackingService.Application.Models.DTOs;

namespace ProgressTrackingService.Presentation.Controllers
{
    [ApiController]
    [Route("progress")]
    [Authorize]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;
        private readonly ILogger<ProgressController> _logger;

        public ProgressController(IProgressService progressService, ILogger<ProgressController> logger)
        {
            _progressService = progressService;
            _logger = logger;
        }

        [HttpPost("lessons/complete")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MarkLessonComplete([FromBody] MarkLessonCompleteDto dto, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                await _progressService.MarkLessonCompleteAsync(userId.Value, dto, ct);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark lesson complete");
                return StatusCode(500, new { message = "Failed to record progress." });
            }
        }

        [HttpGet("courses/{courseId}/lessons")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetCourseLessonProgress(int courseId, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var result = await _progressService.GetCourseLessonProgressAsync(userId.Value, courseId, ct);
            return Ok(result);
        }

        [HttpGet("courses/{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetCourseProgress(int courseId, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var result = await _progressService.GetCourseProgressAsync(userId.Value, courseId, ct);
            if (result == null) return NotFound(new { message = "No progress found for this course." });
            return Ok(result);
        }

        [HttpGet("courses")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetAllProgress(CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var result = await _progressService.GetAllProgressAsync(userId.Value, ct);
            return Ok(result);
        }

        [HttpGet("users/{userId}/courses")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> GetUserProgress(int userId, CancellationToken ct)
        {
            var result = await _progressService.GetAllProgressAsync(userId, ct);
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
