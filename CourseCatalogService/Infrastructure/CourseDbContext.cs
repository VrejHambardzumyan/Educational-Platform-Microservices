using CourseCatalogService.Infrastructure.Configuration;
using CourseCatalogService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
namespace CourseCatalogService.Infrastructure
{
    public class CourseDbContext : DbContext
    {
        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options){}

        public DbSet<Course> Courses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CourseConfiguration());
        }
    }

    //public class CourseDbContext(DbContextOptions<CourseDbContext> options) : DbContext(options)
    //{
    //    public DbSet<Course> Courses { get; set; }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.ApplyConfiguration(new CourseConfiguration());
    //    }
    //}
}   
