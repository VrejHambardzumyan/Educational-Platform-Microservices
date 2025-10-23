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
        [Authorize]
        public async Task<IActionResult> AddCourse([FromBody] CourseRequestDto dto)
        {
            var created = await _courseService.AddCourseAsync(dto);
            return Ok(created);
        }

        [HttpGet("GetAllCourses")]
        [Authorize]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _courseService.GetCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("GetCourseById/{id}")] 
        [Authorize]
        public async Task<IActionResult> GetCourse(int id, CancellationToken cancellationToken)
        {
            var course = await _courseService.GetCourseByIdAsync(id, cancellationToken);
            if (course == null) return NotFound();
            return Ok(course);
        }
    }
}
