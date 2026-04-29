using Microsoft.Extensions.Options;
using PaymentService.Application.Interfaces;
using PaymentService.Application.Models;
using PaymentService.Infrastructure.Configuration;
using PaymentService.Infrastructure.Entities;
using PaymentService.Infrastructure.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PaymentService.Application.Services
{
    public class PaymentProcessor(
        IPaymentRepository repo,
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory,
        IOptions<EnrollmentCallbackSettings> callbackOptions,
        ILogger<PaymentProcessor> logger) : IPaymentProcessor
    {
        private readonly IPaymentRepository _repo = repo;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly EnrollmentCallbackSettings _callback = callbackOptions.Value;
        private readonly ILogger<PaymentProcessor> _logger = logger;

        public async Task<PaymentResponse> ProcessAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default)
        {
            if (request.TotalAmount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero.");

            var existing = await _repo.GetByPaymentIdAsync(request.PaymentId, cancellationToken);
            if (existing != null)
                throw new InvalidOperationException($"Payment {request.PaymentId} already exists.");

            // Step 1: Create PaymentIntent (Stripe equivalent)
            var chargeId = $"ch_{Guid.NewGuid():N}";
            var record = new PaymentRecord
            {
                PaymentId = request.PaymentId,
                UserId = request.UserId,
                Amount = request.TotalAmount,
                Status = PaymentStatus.Processing,
                ChargeId = chargeId,
                CreatedAt = DateTime.UtcNow
            };
            await _repo.AddAsync(record, cancellationToken);

            _logger.LogInformation("PaymentIntent created: {ChargeId} for PaymentId {PaymentId}", chargeId, request.PaymentId);

            // Fire async Stripe simulation — return immediately (202-style)
            _ = Task.Run(() => SimulateStripeProcessingAsync(record.PaymentId, record.ChargeId!));

            return new PaymentResponse
            {
                PaymentId = record.PaymentId,
                UserId = record.UserId,
                Status = record.Status,
                ChargeId = record.ChargeId
            };
        }

        public async Task<PaymentStatusResponse?> GetStatusAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            var record = await _repo.GetByPaymentIdAsync(paymentId, cancellationToken);
            if (record == null) return null;

            return new PaymentStatusResponse
            {
                PaymentId = record.PaymentId,
                Status = record.Status,
                ChargeId = record.ChargeId,
                FailureReason = record.FailureReason,
                CreatedAt = record.CreatedAt,
                ProcessedAt = record.ProcessedAt
            };
        }

        private async Task SimulateStripeProcessingAsync(Guid paymentId, string chargeId)
        {
            try
            {
                // Step 2: Card verification network call (Stripe → card network)
                _logger.LogInformation("[{ChargeId}] Step 2: Card verification...", chargeId);
                await Task.Delay(200);

                // Step 3: Bank authorization request
                _logger.LogInformation("[{ChargeId}] Step 3: Bank authorization...", chargeId);
                await Task.Delay(300);

                // Step 4: Settlement (bank → Stripe → merchant)
                _logger.LogInformation("[{ChargeId}] Step 4: Settlement...", chargeId);
                await Task.Delay(1000);

                var isSuccess = Random.Shared.NextDouble() > 0.1; // 90% success
                var failureReason = isSuccess ? null : "Insufficient funds";

                _logger.LogInformation("[{ChargeId}] Outcome: {Result}", chargeId, isSuccess ? "Completed" : "Failed");

                // Step 5: Persist result in a fresh scope (original scope is already disposed)
                using (var scope = _scopeFactory.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
                    var record = await repo.GetByPaymentIdAsync(paymentId);
                    if (record != null)
                    {
                        record.Status = isSuccess ? PaymentStatus.Completed : PaymentStatus.Failed;
                        record.ProcessedAt = DateTime.UtcNow;
                        record.FailureReason = failureReason;
                        await repo.UpdateAsync(record);
                    }
                }

                // Step 6: Webhook callback to CourseEnrollment (Stripe webhook equivalent)
                await FireCallbackAsync(paymentId, isSuccess, failureReason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stripe simulation failed for PaymentId {PaymentId}", paymentId);
            }
        }

        private async Task FireCallbackAsync(Guid paymentId, bool isSuccess, string? failureReason)
        {
            try
            {
                var payload = new { PaymentId = paymentId, IsSuccess = isSuccess, FailureReason = failureReason };
                var body = JsonSerializer.Serialize(payload);
                var signature = ComputeHmac(body, _callback.WebhookSecret);
                var url = $"{_callback.BaseUrl}/CourseEnrollment/PaymentCallback";

                _logger.LogInformation("[Webhook] Sending to {Url} | BodyLen={Len} | Sig={Sig} | Body={Body}",
                    url, body.Length, signature, body);

                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("X-Webhook-Signature", signature);
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                var client = _httpClientFactory.CreateClient();
                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Webhook callback returned {StatusCode}: {Body}", (int)response.StatusCode, errorBody);
                }
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook callback failed for PaymentId {PaymentId}", paymentId);
            }
        }

        private static string ComputeHmac(string body, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var bodyBytes = Encoding.UTF8.GetBytes(body);
            using var hmac = new HMACSHA256(keyBytes);
            return Convert.ToHexString(hmac.ComputeHash(bodyBytes)).ToLowerInvariant();
        }
    }
}
