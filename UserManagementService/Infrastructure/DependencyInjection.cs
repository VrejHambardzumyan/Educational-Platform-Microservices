using Microsoft.EntityFrameworkCore;

namespace UserManagementService.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddPostgresDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<UserDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
        }
    }
}
