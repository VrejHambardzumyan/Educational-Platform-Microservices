using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Infrastructure.Entities;

namespace ProgressTrackingService.Infrastructure.Configuration
{
    public class CourseProgressConfiguration : IEntityTypeConfiguration<CourseProgress>
    {
        public void Configure(EntityTypeBuilder<CourseProgress> builder)
        {
            builder.ToTable("CourseProgress");
            builder.HasKey(cp => cp.Id);
            builder.HasIndex(cp => new { cp.UserId, cp.CourseId }).IsUnique();
            builder.Ignore(cp => cp.ProgressPercentage);
            builder.Property(cp => cp.CreatedAt).HasDefaultValueSql("NOW()");
        }
    }
}
