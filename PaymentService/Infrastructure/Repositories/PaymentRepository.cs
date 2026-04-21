using Microsoft.EntityFrameworkCore;
using PaymentService.Infrastructure.Entities;
using PaymentService.Infrastructure.Interfaces;

namespace PaymentService.Infrastructure.Repositories
{
    public class PaymentRepository(PaymentDbContext context) : IPaymentRepository
    {
        private readonly PaymentDbContext _context = context;

        public async Task<PaymentRecord> AddAsync(PaymentRecord record, CancellationToken cancellationToken = default)
        {
            _context.Payments.Add(record);
            await _context.SaveChangesAsync(cancellationToken);
            return record;
        }

        public async Task<PaymentRecord?> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default) =>
            await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId, cancellationToken);

        public async Task UpdateAsync(PaymentRecord record, CancellationToken cancellationToken = default)
        {
            _context.Payments.Update(record);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
