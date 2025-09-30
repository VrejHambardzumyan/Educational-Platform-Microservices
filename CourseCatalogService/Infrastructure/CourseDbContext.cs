using CourseCatalogService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
namespace CourseCatalogService.Infrastructure
{
    public class CourseDbContext(DbContextOptions<CourseDbContext> options) : DbContext(options)
    {
        public DbSet<Course> Courses { get; set; }
    }
}   
