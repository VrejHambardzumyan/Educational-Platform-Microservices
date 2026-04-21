using System.Collections.Concurrent;
using UserManagementService.Infrastructure.Entities;
using UserManagementService.Infrastructure.Interfaces;

namespace UserManagementService.Infrastructure.Repositories
{
    public class MockUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<string, User> _users = new();
        private readonly ConcurrentDictionary<string, RefreshToken> _refreshTokens = new();
        private int _nextId = 1;

        public Task<User?> GetByUserNameAsync(string userName)
        {
            _users.TryGetValue(userName, out var user);
            return Task.FromResult(user);
        }

        public Task AddEntityAsync(User entity)
        {
            entity.Id = Interlocked.Increment(ref _nextId);
            _users[entity.UserName] = entity;
            return Task.CompletedTask;
        }

        public Task<User?> GetByIdAsync(int id)
        {
            var user = _users.Values.FirstOrDefault(u => u.Id == id && !u.IsDeleted);
            return Task.FromResult(user);
        }

        public Task<(IEnumerable<User> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
        {
            var all = _users.Values.Where(u => !u.IsDeleted).ToList();
            var items = all.Skip((page - 1) * pageSize).Take(pageSize);
            return Task.FromResult((items.AsEnumerable(), all.Count));
        }

        public Task UpdateAsync(User entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _users[entity.UserName] = entity;
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() => Task.CompletedTask;

        public Task AddRefreshTokenAsync(RefreshToken token)
        {
            _refreshTokens[token.TokenHash] = token;
            return Task.CompletedTask;
        }

        public Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash)
        {
            _refreshTokens.TryGetValue(tokenHash, out var token);
            if (token != null && !token.IsRevoked)
            {
                token.User = _users.Values.First(u => u.Id == token.UserId);
                return Task.FromResult<RefreshToken?>(token);
            }
            return Task.FromResult<RefreshToken?>(null);
        }

        public Task RevokeRefreshTokenAsync(RefreshToken token, string? replacedByHash = null)
        {
            token.IsRevoked = true;
            token.ReplacedByTokenHash = replacedByHash;
            return Task.CompletedTask;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = _users.Values.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
            return Task.FromResult(user);
        }

        private readonly ConcurrentDictionary<int, OtpRecord> _otps = new();
        private int _otpId = 1;

        public Task AddOtpAsync(OtpRecord otp, CancellationToken cancellationToken = default)
        {
            otp.Id = Interlocked.Increment(ref _otpId);
            _otps[otp.Id] = otp;
            return Task.CompletedTask;
        }

        public Task<OtpRecord?> GetValidOtpAsync(int userId, string purpose, CancellationToken cancellationToken = default)
        {
            var record = _otps.Values
                .Where(o => o.UserId == userId && o.Purpose == purpose && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();
            return Task.FromResult(record);
        }

        public Task MarkOtpUsedAsync(OtpRecord otp, CancellationToken cancellationToken = default)
        {
            otp.IsUsed = true;
            return Task.CompletedTask;
        }
    }
}
