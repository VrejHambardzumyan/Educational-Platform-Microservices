using CourseEnrollment.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseEnrollment.Infrastructure.Configuration
{
    public class EnrollConfiguration : IEntityTypeConfiguration<EnrollmentEntity>
    {
        public void Configure(EntityTypeBuilder<EnrollmentEntity> builder)
        {
            builder.ToTable("Enrollment");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            builder.Property(e => e.CourseId)
                .HasColumnName("CourseId")
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .HasColumnName("CreatedAt");

            builder.Property(e => e.Amount)
                .HasColumnName("Amount");

            builder.Property(e => e.ActivatedAt)
                .HasColumnName("ActivatedAt");

            builder.Property(e => e.Status)
                .HasColumnName("PaymentStatus");
        }
    }
}
