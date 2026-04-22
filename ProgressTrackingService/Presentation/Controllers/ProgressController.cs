using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Application.Interfaces;
using ProgressTrackingService.Application.Models.DTOs;

namespace ProgressTrackingService.Presentation.Controllers
{
    [ApiController]
    [Route("progress")]
    [Authorize]
    public class ProgressController(IProgressService progressService, ILogger<ProgressController> logger)
        : ControllerBase
    {
        private readonly IProgressService _progressService = progressService;
        private readonly ILogger<ProgressController> _logger = logger;

        /// <summary>
        /// Marks a lesson as complete for the authenticated student.
        /// Validates enrollment and lesson ownership via cross-service calls before persisting.
        /// </summary>
        [HttpPost("lessons/complete")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MarkLessonComplete(
            [FromBody] MarkLessonCompleteDto dto, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            try
            {
                await _progressService.MarkLessonCompleteAsync(userId.Value, dto, ct);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark lesson {LessonId} complete for user {UserId}",
                    dto.LessonId, userId);
                return StatusCode(500, new { message = "Failed to record progress." });
            }
        }

        /// <summary>Returns the completion state of every lesson the student has interacted with in a course.</summary>
        [HttpGet("courses/{courseId}/lessons")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetCourseLessonProgress(int courseId, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            var result = await _progressService.GetCourseLessonProgressAsync(userId.Value, courseId, ct);
            return Ok(result);
        }

        /// <summary>Returns the aggregated progress (%, isCompleted) for a single course.</summary>
        [HttpGet("courses/{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetCourseProgress(int courseId, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            var result = await _progressService.GetCourseProgressAsync(userId.Value, courseId, ct);
            if (result is null) return NotFound(new { message = "No progress found for this course." });
            return Ok(result);
        }

        /// <summary>Returns the progress summary for every course the student has started.</summary>
        [HttpGet("courses")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetAllProgress(CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            var result = await _progressService.GetAllProgressAsync(userId.Value, ct);
            return Ok(result);
        }

        /// <summary>Returns all course progress for a specific user. Admin and Instructor only.</summary>
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
            return claim is not null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }
}
