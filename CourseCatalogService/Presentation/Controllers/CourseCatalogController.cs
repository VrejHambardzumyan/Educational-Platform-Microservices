using Microsoft.AspNetCore.Mvc;
using CourseCatalogService.Application.Models.DTOs;
using CourseCatalogService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CourseCatalogService.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseCatalogController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseCatalogController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost("AddCourse")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> AddCourse([FromBody] CourseRequestDto dto)
        {
            var instructorId = GetCurrentUserId();
            if (instructorId == null) return Unauthorized();

            var created = await _courseService.AddCourseAsync(dto, instructorId.Value);
            return CreatedAtAction(nameof(GetCourse), new { id = created.Id }, created);
        }

        [HttpGet("GetAllCourses")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourses(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? category = null,
            [FromQuery] string? search = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var courses = await _courseService.GetCoursesAsync(page, pageSize, category, search);
            return Ok(courses);
        }

        [HttpGet("GetCourseById/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourse(int id, CancellationToken cancellationToken)
        {
            var course = await _courseService.GetCourseByIdAsync(id, cancellationToken);
            if (course == null) return NotFound(new { message = $"Course with ID {id} not found." });
            return Ok(course);
        }

        [HttpGet("instructor/{instructorId}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> GetByInstructor(int instructorId, CancellationToken cancellationToken)
        {
            var courses = await _courseService.GetByInstructorIdAsync(instructorId, cancellationToken);
            return Ok(courses);
        }

        [HttpGet("my-courses")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetMyCourses(CancellationToken cancellationToken)
        {
            var instructorId = GetCurrentUserId();
            if (instructorId == null) return Unauthorized();

            var courses = await _courseService.GetByInstructorIdAsync(instructorId.Value, cancellationToken);
            return Ok(courses);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseUpdateRequestDto dto)
        {
            var instructorId = GetCurrentUserId();
            if (instructorId == null) return Unauthorized();

            var updated = await _courseService.UpdateCourseAsync(id, dto, instructorId.Value);
            if (updated == null) return NotFound(new { message = "Course not found or you don't own it." });
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var isAdmin = User.IsInRole("Admin");

            var result = await _courseService.DeleteCourseAsync(id, userId.Value, isAdmin);
            if (!result) return NotFound(new { message = "Course not found or not authorized." });
            return NoContent();
        }

        [HttpPatch("{id}/publish")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> PublishCourse(int id)
        {
            var instructorId = GetCurrentUserId();
            if (instructorId == null) return Unauthorized();

            var result = await _courseService.PublishCourseAsync(id, instructorId.Value);
            if (!result) return NotFound(new { message = "Course not found or you don't own it." });
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
