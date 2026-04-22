using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Infrastructure.Entities;

namespace PaymentService.Infrastructure.Configuration
{
    public class PaymentRecordConfiguration : IEntityTypeConfiguration<PaymentRecord>
    {
        public void Configure(EntityTypeBuilder<PaymentRecord> builder)
        {
            builder.ToTable("payments");

            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.PaymentId).IsUnique();

            builder.Property(p => p.PaymentId)
                .IsRequired();

            builder.Property(p => p.UserId)
                .IsRequired();

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.ChargeId)
                .HasMaxLength(100);

            builder.Property(p => p.FailureReason)
                .HasMaxLength(500);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("NOW()");
        }
    }
}
