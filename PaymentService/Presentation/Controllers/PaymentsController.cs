using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Interfaces;
using PaymentService.Application.Models;

namespace PaymentService.Presentation.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController(IPaymentProcessor processor, ILogger<PaymentsController> logger) : ControllerBase
    {
        private readonly IPaymentProcessor _processor = processor;
        private readonly ILogger<PaymentsController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _processor.ProcessAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment processing failed for PaymentId {PaymentId}", request.PaymentId);
                return StatusCode(500, new { message = "Payment processing failed." });
            }
        }

        [HttpGet("{paymentId:guid}/status")]
        public async Task<IActionResult> GetStatus(Guid paymentId, CancellationToken cancellationToken)
        {
            var result = await _processor.GetStatusAsync(paymentId, cancellationToken);
            if (result == null) return NotFound(new { message = "Payment not found." });
            return Ok(result);
        }
    }
}
