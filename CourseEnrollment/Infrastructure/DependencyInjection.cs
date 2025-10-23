using Microsoft.EntityFrameworkCore;

namespace CourseEnrollment.Infrastructure
{
    public static class DependencyInjection 
    {
        public static void AddPostgresDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EnrollmentDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
