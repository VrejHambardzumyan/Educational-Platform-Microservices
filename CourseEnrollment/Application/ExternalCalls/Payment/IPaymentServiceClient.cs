using static CourseEnrollment.Application.ExternalCalls.PaymentServiceClient;

namespace CourseEnrollment.Application.ExternalCalls.Payment
{
    public interface IPaymentServiceClient
    {
        Task<PaymentServiceResponseDto> CreatPaymentAsync(int userId, Guid paymentId, decimal totalAmount, CancellationToken cancellationToken = default);
    }
}
    