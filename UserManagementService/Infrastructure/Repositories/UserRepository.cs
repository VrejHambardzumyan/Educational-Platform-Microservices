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

        public async Task<User?> GetByUserNameAsync(string userName) =>
            await WithRoleAndPermissions()
                .FirstOrDefaultAsync(u => u.UserName == userName && !u.IsDeleted);

        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users
                .Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        public async Task<(IEnumerable<User> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
        {
            var query = _context.Users
                .Include(u => u.UserRole)
                .Where(u => !u.IsDeleted)
                .AsNoTracking();
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

        public async Task DeleteAsync(User entity)
        {
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRefreshTokenAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash) =>
            await _context.RefreshTokens
                .Include(r => r.User)
                    .ThenInclude(u => u.UserRole)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.TokenHash == tokenHash && !r.IsRevoked);

        public async Task RevokeRefreshTokenAsync(RefreshToken token, string? replacedByHash = null)
        {
            token.IsRevoked = true;
            token.ReplacedByTokenHash = replacedByHash;
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            await WithRoleAndPermissions()
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);

        public async Task AddOtpAsync(OtpRecord otp, CancellationToken cancellationToken = default)
        {
            _context.OtpRecords.Add(otp);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<OtpRecord?> GetValidOtpAsync(int userId, string purpose, CancellationToken cancellationToken = default) =>
            await _context.OtpRecords
                .Where(o => o.UserId == userId && o.Purpose == purpose && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task MarkOtpUsedAsync(OtpRecord otp, CancellationToken cancellationToken = default)
        {
            otp.IsUsed = true;
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Eagerly loads role + full permission chain — required for JWT generation.
        private IQueryable<User> WithRoleAndPermissions() =>
            _context.Users
                .Include(u => u.UserRole)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission);
    }
}
