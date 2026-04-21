using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Application.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CourseCatalogService.Presentation.Controllers
{
    [ApiController]
    [Route("courses/{courseId}/lessons")]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        private readonly ILogger<LessonsController> _logger;

        public LessonsController(ILessonService lessonService, ILogger<LessonsController> logger)
        {
            _lessonService = lessonService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetLessons(int courseId, CancellationToken cancellationToken)
        {
            var lessons = await _lessonService.GetByCourseIdAsync(courseId, cancellationToken);
            return Ok(lessons);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLesson(int courseId, int id, CancellationToken cancellationToken)
        {
            var lesson = await _lessonService.GetByIdAsync(id, cancellationToken);
            if (lesson == null || lesson.CourseId != courseId) return NotFound();
            return Ok(lesson);
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> AddLesson(int courseId, [FromBody] LessonRequestDto dto)
        {
            var instructorId = GetCurrentUserId();
            if (instructorId == null) return Unauthorized();

            try
            {
                var created = await _lessonService.AddLessonAsync(courseId, dto, instructorId.Value);
                return CreatedAtAction(nameof(GetLesson), new { courseId, id = created.Id }, created);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add lesson");
                return StatusCode(500, new { message = "Failed to add lesson." });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateLesson(int courseId, int id, [FromBody] LessonRequestDto dto)
        {
            var instructorId = GetCurrentUserId();
            if (instructorId == null) return Unauthorized();

            var updated = await _lessonService.UpdateLessonAsync(id, dto, instructorId.Value);
            if (updated == null) return NotFound(new { message = "Lesson not found or you don't own this course." });
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> DeleteLesson(int courseId, int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var result = await _lessonService.DeleteLessonAsync(id, userId.Value, User.IsInRole("Admin"));
            if (!result) return NotFound(new { message = "Lesson not found or not authorized." });
            return NoContent();
        }

        [HttpPut("{lessonId}/video-url")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateVideoUrl(int courseId, int lessonId, [FromBody] VideoUrlUpdateDto dto, CancellationToken cancellationToken)
        {
            var instructorId = GetCurrentUserId();
            if (instructorId == null) return Unauthorized();

            var updated = await _lessonService.UpdateVideoUrlAsync(courseId, lessonId, dto.VideoUrl, instructorId.Value, cancellationToken);
            if (updated == null) return NotFound(new { message = "Lesson not found or you don't own this course." });
            return Ok(updated);
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
