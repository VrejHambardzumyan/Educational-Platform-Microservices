using Microsoft.EntityFrameworkCore;
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
    }
}
