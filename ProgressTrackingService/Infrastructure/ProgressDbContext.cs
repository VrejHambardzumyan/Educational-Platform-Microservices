using Microsoft.EntityFrameworkCore;
using ProgressTrackingService.Infrastructure.Configuration;
using ProgressTrackingService.Infrastructure.Entities;

namespace ProgressTrackingService.Infrastructure
{
    public class ProgressDbContext : DbContext
    {
        public ProgressDbContext(DbContextOptions<ProgressDbContext> options) : base(options) { }

        public DbSet<LessonProgress> LessonProgress { get; set; }
        public DbSet<CourseProgress> CourseProgress { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("progress_tracking");

            modelBuilder.ApplyConfiguration(new LessonProgressConfiguration());
            modelBuilder.ApplyConfiguration(new CourseProgressConfiguration());
        }
    }
}
