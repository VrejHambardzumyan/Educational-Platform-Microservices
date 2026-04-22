using Microsoft.EntityFrameworkCore;
using UserManagementService.Infrastructure.Entities;

namespace UserManagementService.Infrastructure.Seeders
{
    public static class UserSeeder
    {
        /// <summary>Seeds the default admin account if no Admin user exists yet.</summary>
        public static async Task SeedAsync(UserDbContext context)
        {
            if (await context.Users.AnyAsync(u => u.RoleId == RoleIds.Admin))
                return;

            var admin = new User
            {
                UserName = "admin",
                Email = "admin@platform.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin@123456"),
                RoleId = RoleIds.Admin
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
