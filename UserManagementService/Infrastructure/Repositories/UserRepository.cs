using Microsoft.EntityFrameworkCore;
using UserManagementService.Infrastructure.Entities;
using UserManagementService.Infrastructure.Interfaces;

namespace UserManagementService.Infrastructure.Repositories
{
    public class UserRepository(UserDbContext dbContext) : IUserRepository
    {
        private readonly UserDbContext _context = dbContext;

        public async Task AddEntityAsync(User entity)
        {
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName && !u.IsDeleted);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<(IEnumerable<User> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
        {
            var query = _context.Users.Where(u => !u.IsDeleted).AsNoTracking();
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, totalCount);
        }

        public async Task UpdateAsync(User entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task AddRefreshTokenAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.TokenHash == tokenHash && !r.IsRevoked);
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken token, string? replacedByHash = null)
        {
            token.IsRevoked = true;
            token.ReplacedByTokenHash = replacedByHash;
            await _context.SaveChangesAsync();
        }
    }
}
