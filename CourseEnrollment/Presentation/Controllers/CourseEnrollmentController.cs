using CourseEnrollment.Application.Interfaces;
using CourseEnrollment.Application.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Middleware;

namespace CourseEnrollment.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
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
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AddEnrollmentAsync([FromBody] CreateEnrollmentRequestDto requestDto, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            requestDto.UserId = userId.Value;
            var result = await _enrollment.AddEnrollmentAsync(requestDto, cancellationToken);
            return Ok(result);
        }

        [HttpPut("Activate/{id}")]
        public async Task<IActionResult> ActivateAsync(int id, CancellationToken cancellationToken)
        {
            await _enrollment.MarkAsPaidAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPut("Cancel/{id}")]
        public async Task<IActionResult> CancelAsync(int id, CancellationToken cancellationToken)
        {
            await _enrollment.MarkAsDeletedAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Returns whether the current user has an active (paid) enrollment for the given course.
        /// Called by ProgressTrackingService before recording lesson progress.
        /// </summary>
        [HttpGet("is-enrolled/{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> IsEnrolled(int courseId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var enrollments = await _enrollment.GetAllByUserIdAsync(userId.Value, cancellationToken);
            var isEnrolled = enrollments.Any(e => e.CourseId == courseId && e.ActivatedAt.HasValue);
            return Ok(new { isEnrolled });
        }

        [HttpGet("my-enrollments")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyEnrollments(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var result = await _enrollment.GetAllByUserIdAsync(userId.Value, cancellationToken);
            return Ok(result);
        }

        [HttpGet("GetUser/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            var result = await _enrollment.GetAllByUserIdAsync(userId, cancellationToken);
            return Ok(result ?? Enumerable.Empty<EnrollmentResponseDto>());
        }

        [HttpGet("GetEnrollment/{id}")]
        public async Task<IActionResult> GetByEnrollmentIdAsync(int id, CancellationToken cancellationToken)
        {
            var result = await _enrollment.GetByIdAsync(id, cancellationToken);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("SubmitCard")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitCard([FromBody] SubmitCardRequestDto cardDto, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var paymentId = await _enrollment.SubmitCardAsync(userId.Value, cardDto, cancellationToken);
            return Ok(new { paymentId });
        }

        [HttpPost("PaymentCallback")]
        [AllowAnonymous]
        [ValidateWebhookSignature]
        public async Task<IActionResult> PaymentCallback([FromBody] PaymentCallbackDto dto, CancellationToken cancellationToken)
        {
            await _enrollment.HandlePaymentCallbackAsync(dto.PaymentId, dto.IsSuccess, cancellationToken);
            return Ok(new { message = "Payment status updated successfully" });
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
