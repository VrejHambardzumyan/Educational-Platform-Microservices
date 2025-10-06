using Microsoft.AspNetCore.Mvc;
using CourseCatalogService.Application.Models.DTOs;
using CourseCatalogService.Application.Interfaces;

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
        public async Task<IActionResult> AddCourse([FromBody] CourseRequestDto dto)
        {
            var created = await _courseService.AddCourseAsync(dto);
            return Ok(created);
        }

        [HttpGet("GetAllCourses")]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _courseService.GetCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("GetCourseByName")]
        public async Task<IActionResult> GetCourse(string courseName, CancellationToken cancellationToken)
        {
            var course = await _courseService.GetCourseByNameAsync(courseName, cancellationToken);
            if (course == null) return NotFound();
            return Ok(course);
        }
    }
}
