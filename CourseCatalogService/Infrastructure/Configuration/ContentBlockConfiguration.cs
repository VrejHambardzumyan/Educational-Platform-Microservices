using CourseCatalogService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseCatalogService.Infrastructure.Configuration
{
    public class ContentBlockConfiguration : IEntityTypeConfiguration<ContentBlock>
    {
        public void Configure(EntityTypeBuilder<ContentBlock> builder)
        {
            builder.ToTable("content_blocks");
            builder.HasKey(cb => cb.Id);

            builder.Property(cb => cb.Type)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(cb => cb.VideoUrl)
                .HasMaxLength(1000);

            builder.Property(cb => cb.FileUrl)
                .HasMaxLength(1000);

            builder.Property(cb => cb.Metadata)
                .HasColumnType("jsonb");

            builder.HasOne(cb => cb.Lesson)
                .WithMany(l => l.ContentBlocks)
                .HasForeignKey(cb => cb.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(cb => new { cb.LessonId, cb.Order });
        }
    }
}
