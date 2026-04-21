using CourseCatalogService.Infrastructure.Configuration;
using CourseCatalogService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
namespace CourseCatalogService.Infrastructure
{
    public class CourseDbContext : DbContext
    {
        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options){}

        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CourseConfiguration());
            modelBuilder.ApplyConfiguration(new LessonConfiguration());
        }
    }
}   
