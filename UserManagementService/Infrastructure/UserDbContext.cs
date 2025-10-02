using Microsoft.EntityFrameworkCore;
using UserManagementService.Infrastructure.Configuration;
using UserManagementService.Infrastructure.Entities;

namespace UserManagementService.Infrastructure
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }     
}
