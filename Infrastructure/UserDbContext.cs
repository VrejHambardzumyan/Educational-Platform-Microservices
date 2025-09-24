using Microsoft.EntityFrameworkCore;
using UserManagementService.Infrastructure.Entities;

namespace UserManagementService.Infrastructure
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options){}
        public DbSet<User> Users { get; set; }
         
    }
       

}
