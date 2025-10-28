using CourseEnrollment.Application.Interfaces;
using CourseEnrollment.Application.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CourseEnrollment.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseEnrollmentController : ControllerBase
    {
        private readonly ILogger<CourseEnrollmentController> _logger;

        private readonly IEnrollmentService _enrollment;
        public CourseEnrollmentController(ILogger<CourseEnrollmentController> logger, IEnrollmentService enrollment)
        {
            _logger = logger;
            _enrollment = enrollment;
        }

        [HttpPost("AddEnrollment")]
        public async Task<IActionResult> AddEnrollmentAsync([FromBody]CreateEnrollmentRequestDto requestDto ,CancellationToken cancellationToken)
        {
            var result = await _enrollment.AddEnrollmentAsync(requestDto, cancellationToken);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("Activate/{id}")]
        public async Task<IActionResult> ActivateAsync(int id, CancellationToken cancellationToken)
        {
            await _enrollment.MarkAsPaidAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPut("Cancel/{id}")]
        public async Task<IActionResult> CancelAync(int id, CancellationToken cancellationToken)
        {
            await _enrollment.MarkAsDeletedAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("GetUser/{userId}")]
        public async Task<IActionResult> GetByUserIdAsync(int userid, CancellationToken cancellationToken)
        {
            var result = await _enrollment.GetAllByUserIdAsync(userid, cancellationToken);
            return Ok(result);
        }

        [HttpGet("GetEnrollment/{userId}")]
        public async Task<IActionResult> GetByEnrollmnetIdAsync(int userid, CancellationToken cancellationToken)
        {
            var result = await _enrollment.GetByIdAsync(userid, cancellationToken);
            return Ok(result);
        }
    }
}
