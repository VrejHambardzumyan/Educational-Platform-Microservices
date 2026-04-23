using CourseEnrollment.Application.ExternalCalls.Payment;
using Microsoft.Extensions.Options;
using Shared.Authentication;

namespace CourseEnrollment.Application.ExternalCalls
{
    public class PaymentServiceClient(
        HttpClient httpClient,
        IOptions<PaymentServiceSettings> settings,
        IServiceTokenProvider tokenProvider) : IPaymentServiceClient
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly PaymentServiceSettings _settings = settings.Value;
        private readonly IServiceTokenProvider _tokenProvider = tokenProvider;

        public async Task<PaymentServiceResponseDto> CreatPaymentAsync(int userId, Guid paymentId, decimal totalAmount, CancellationToken cancellationToken = default)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.GenerateToken());

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
