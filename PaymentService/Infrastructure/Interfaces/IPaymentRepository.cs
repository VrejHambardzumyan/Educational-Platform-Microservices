using PaymentService.Infrastructure.Entities;

namespace PaymentService.Infrastructure.Interfaces
{
    public interface IPaymentRepository
    {
        Task<PaymentRecord> AddAsync(PaymentRecord record, CancellationToken cancellationToken = default);
        Task<PaymentRecord?> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
        Task UpdateAsync(PaymentRecord record, CancellationToken cancellationToken = default);
    }
}
