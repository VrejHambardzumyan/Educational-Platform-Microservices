using Microsoft.EntityFrameworkCore;
using PaymentService.Infrastructure.Configuration;
using PaymentService.Infrastructure.Entities;

namespace PaymentService.Infrastructure
{
    public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
    {
        public DbSet<PaymentRecord> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("payment_service");
            modelBuilder.ApplyConfiguration(new PaymentRecordConfiguration());
        }
    }
}
