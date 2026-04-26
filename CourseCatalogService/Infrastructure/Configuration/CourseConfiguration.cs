using Microsoft.EntityFrameworkCore;
using CourseCatalogService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseCatalogService.Infrastructure.Configuration
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("courses");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(c => c.DurationInMonth)
                .IsRequired();

            builder.Property(c => c.Price)
                .IsRequired();

            builder.Property(c => c.InstructorId)
                .IsRequired();

            builder.Property(c => c.Status)
                .HasConversion<string>()
                .HasDefaultValue(CourseStatus.Draft);

            builder.Property(c => c.Category)
                .HasMaxLength(100);

            builder.Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(c => c.AverageRating)
                .HasDefaultValue(0.0);

            builder.Property(c => c.RatingCount)
                .HasDefaultValue(0);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("NOW()");

            builder.HasIndex(c => c.InstructorId);
            builder.HasIndex(c => c.Status);
            builder.HasIndex(c => c.Category);

            builder.Property(c => c.SearchVector)
                .HasColumnName("search_vector")
                .HasComputedColumnSql(
                    "to_tsvector('english', coalesce(Title,'') || ' ' || coalesce(description,''))",
                    stored: true);

            builder.HasIndex(c => c.SearchVector)
                .HasMethod("GIN");
        }
    }
}
