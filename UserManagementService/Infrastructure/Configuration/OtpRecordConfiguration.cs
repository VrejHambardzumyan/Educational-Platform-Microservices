using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagementService.Infrastructure.Entities;

namespace UserManagementService.Infrastructure.Configuration
{
    public class OtpRecordConfiguration : IEntityTypeConfiguration<OtpRecord>
    {
        public void Configure(EntityTypeBuilder<OtpRecord> builder)
        {
            builder.ToTable("otp_records");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.OtpCodeHash).IsRequired().HasMaxLength(100);
            builder.Property(o => o.Purpose).IsRequired().HasMaxLength(30);
            builder.Property(o => o.ExpiresAt).IsRequired();
            builder.Property(o => o.IsUsed).HasDefaultValue(false);
            builder.Property(o => o.CreatedAt).HasDefaultValueSql("NOW()");
            builder.HasIndex(o => new { o.UserId, o.Purpose });
            builder.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
