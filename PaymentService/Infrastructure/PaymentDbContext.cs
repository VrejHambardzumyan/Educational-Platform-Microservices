using Microsoft.EntityFrameworkCore;
using PaymentService.Infrastructure.Entities;

namespace PaymentService.Infrastructure
{
    public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
    {
        public DbSet<PaymentRecord> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentRecord>(entity =>
            {
                entity.ToTable("payments");
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.PaymentId).IsUnique();
                entity.Property(p => p.PaymentId).IsRequired();
                entity.Property(p => p.Amount).HasPrecision(18, 2);
                entity.Property(p => p.Status).HasMaxLength(20).IsRequired();
                entity.Property(p => p.ChargeId).HasMaxLength(100);
                entity.Property(p => p.FailureReason).HasMaxLength(500);
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("NOW()");
            });
        }
    }
}
