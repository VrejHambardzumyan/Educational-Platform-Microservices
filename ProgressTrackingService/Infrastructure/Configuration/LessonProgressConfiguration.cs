using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Infrastructure.Entities;

namespace ProgressTrackingService.Infrastructure.Configuration
{
    public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
    {
        public void Configure(EntityTypeBuilder<LessonProgress> builder)
        {
            builder.ToTable("LessonProgress");
            builder.HasKey(lp => lp.Id);
            builder.HasIndex(lp => new { lp.UserId, lp.LessonId }).IsUnique();
            builder.HasIndex(lp => new { lp.UserId, lp.CourseId });
            builder.Property(lp => lp.CreatedAt).HasDefaultValueSql("NOW()");
        }
    }
}
