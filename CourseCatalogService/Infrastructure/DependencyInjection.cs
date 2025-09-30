using Microsoft.EntityFrameworkCore;

namespace CourseCatalogService.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddPostgresDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<CourseDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
        }
    }
}