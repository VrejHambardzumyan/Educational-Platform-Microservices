using CourseEnrollment.Application.ExternalCalls.Payment;
using Microsoft.Extensions.Options;

namespace CourseEnrollment.Application.ExternalCalls
{
    public class PaymentServiceClient : IPaymentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly PaymentServiceSettings _settings;
        public PaymentServiceClient(HttpClient httpClient,IOptions<PaymentServiceSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<PaymentServiceResponseDto> CreatPaymentAsync(int userId, Guid paymentId, decimal totalAmount, CancellationToken cancellationToken = default)
        {
            var request = new
            {
                UserId = userId,
                PaymentId = paymentId,
                TotalAmount = totalAmount,
            };

            var response = await _httpClient.PostAsJsonAsync($"{_settings.BaseUrl}/api/payments", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PaymentServiceResponseDto>(cancellationToken: cancellationToken)
                   ?? throw new InvalidOperationException("Invalid response from Payment Service");
        }

        public class PaymentServiceResponseDto
        {
            public int UserId { get; set; }
            public Guid PaymentId { get; set; }
        }
    }
}
