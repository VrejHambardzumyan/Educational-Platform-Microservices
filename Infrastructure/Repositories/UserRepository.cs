using Microsoft.EntityFrameworkCore;
using UserManagementService.Infrastructure.Entities;
using UserManagementService.Infrastructure.Interfaces;

namespace UserManagementService.Infrastructure.Repositories
{
    public class UserRepository(UserDbContext dbContext) : IUserRepository
    {
        private readonly UserDbContext _context = dbContext;
        
        public async Task AddEntity(User entity)

        {
            //_context.Users.Add(entity);

            //await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }
    }
}
