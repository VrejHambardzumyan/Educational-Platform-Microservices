using CourseEnrollment.Infrastructure.Configuration;
using CourseEnrollment.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollment.Infrastructure
{
    public class EnrollmentDbContext : DbContext
    {
        public EnrollmentDbContext(DbContextOptions<EnrollmentDbContext> options) : base(options){}
    
        public DbSet<EnrollmentEntity> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EnrollConfiguration());
        }
    }
}
