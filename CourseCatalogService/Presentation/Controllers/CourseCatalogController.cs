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
        private readonly ILogger<CourseCatalogController> _logger;

        public CourseCatalogController(ILogger<CourseCatalogController> logger, ICourseService courseService)
        {
            _courseService = courseService;
            _logger = logger;
        }

        [HttpPost("AddCourse")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> AddCourse([FromBody] CourseRequestDto dto)
        {
            try
            {
                var created = await _courseService.AddCourseAsync(dto);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to add course.", detail = ex.Message });
            }
        }

        [HttpGet("GetAllCourses")]
        [Authorize]
        public async Task<IActionResult> GetCourses()
        {
            try
            {
                var courses = await _courseService.GetCoursesAsync();
                return Ok(courses ?? Enumerable.Empty<CourseResponseDto>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve courses.", detail = ex.Message });
            }
        }

        [HttpGet("GetCourseById/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourse(int id, CancellationToken cancellationToken)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id, cancellationToken);
                if (course == null) return NotFound(new { message = $"Course with ID {id} not found." });
                return Ok(course);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve course.", detail = ex.Message });
            }
        }
    }
}
