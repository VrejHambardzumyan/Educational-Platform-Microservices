using Microsoft.EntityFrameworkCore;
    
namespace UserManagementService.Infrastructure
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options){}
        public DbSet<User> Users { get; set; }
         
    }
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }   

}
