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
        public async Task<IActionResult> AddEnrollmentAsync([FromBody] CreateEnrollmentRequestDto requestDto, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _enrollment.AddEnrollmentAsync(requestDto, cancellationToken);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create enrollment.", detail = ex.Message });
            }
        }

        // REPLACE BOTH:
        [HttpPut("Activate/{id}")]
        public async Task<IActionResult> ActivateAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _enrollment.MarkAsPaidAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to activate enrollment.", detail = ex.Message });
            }
        }

        [HttpPut("Cancel/{id}")]
        public async Task<IActionResult> CancelAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _enrollment.MarkAsDeletedAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to cancel enrollment.", detail = ex.Message });
            }
        }

        [HttpGet("GetUser/{userId}")]
        public async Task<IActionResult> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _enrollment.GetAllByUserIdAsync(userId, cancellationToken);
                return Ok(result ?? Enumerable.Empty<EnrollmentResponseDto>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve enrollments.", detail = ex.Message });
            }
        }

        [HttpGet("GetEnrollment/{userId}")]
        public async Task<IActionResult> GetByEnrollmnetIdAsync(int userid, CancellationToken cancellationToken)
        {
            var result = await _enrollment.GetByIdAsync(userid, cancellationToken);
            return Ok(result);
        }

        [HttpPost("InitiatePayment/{userId}")]
        public async Task<IActionResult> InitiatePayment(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var paymentId = await _enrollment.InitiatePaymentAsync(userId, cancellationToken);
                return Ok(new { paymentId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("PaymentCallback")]
        public async Task<IActionResult> PaymentCallback([FromBody] PaymentCallbackDto dto, CancellationToken cancellationToken)
        {
            try
            {
                await _enrollment.HandlePaymentCallbackAsync(dto.PaymentId, dto.IsSuccess, cancellationToken);
                return Ok(new { message = "Payment status updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
