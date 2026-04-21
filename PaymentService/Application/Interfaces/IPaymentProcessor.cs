using PaymentService.Application.Models;

namespace PaymentService.Application.Interfaces
{
    public interface IPaymentProcessor
    {
        Task<PaymentResponse> ProcessAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default);
        Task<PaymentStatusResponse?> GetStatusAsync(Guid paymentId, CancellationToken cancellationToken = default);
    }
}
