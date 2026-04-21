using static CourseEnrollment.Application.ExternalCalls.PaymentServiceClient;

namespace CourseEnrollment.Application.ExternalCalls.Payment
{
    public class MockPaymentServiceClient : IPaymentServiceClient
    {
        public async Task<PaymentServiceResponseDto> CreatPaymentAsync(
            int userId, Guid paymentId, decimal totalAmount, CancellationToken cancellationToken = default)
        {
            if (totalAmount <= 0)
                throw new InvalidOperationException("Payment amount must be greater than zero.");

            // Simulate network latency to a payment gateway
            await Task.Delay(200, cancellationToken);

            return new PaymentServiceResponseDto { UserId = userId, PaymentId = paymentId };
        }
    }
}
