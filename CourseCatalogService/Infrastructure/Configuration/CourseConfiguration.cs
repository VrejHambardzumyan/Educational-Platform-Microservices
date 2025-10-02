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
                .IsRequired();
            
            builder.Property(c => c.Description)
                .IsRequired();

            builder.Property(c => c.DurationInMonth)
                .IsRequired();

            builder.Property(c => c.Price)
                .IsRequired();

            builder.HasIndex(c => c.Id)
                .IsUnique();
        }
    }
}
