using Microsoft.EntityFrameworkCore;
using UserManagementService.Infrastructure.Entities;

namespace UserManagementService.Infrastructure.Seeders
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(UserDbContext context)
        {
            if (await context.Users.AnyAsync(u => u.Role == "Admin"))
                return;

            var admin = new User
            {
                UserName = "admin",
                Email = "admin@platform.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin@123456"),
                Role = "Admin"
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
