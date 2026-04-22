using CourseCatalogService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseCatalogService.Infrastructure.Configuration
{
    public class CourseRatingConfiguration : IEntityTypeConfiguration<CourseRating>
    {
        public void Configure(EntityTypeBuilder<CourseRating> builder)
        {
            builder.ToTable("course_ratings");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(r => r.Feedback)
                .HasMaxLength(1000);

            builder.Property(r => r.CreatedAt)
                .HasDefaultValueSql("NOW()");

            builder.HasOne(r => r.Course)
                .WithMany()
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => new { r.CourseId, r.UserId }).IsUnique();
        }
    }
}
