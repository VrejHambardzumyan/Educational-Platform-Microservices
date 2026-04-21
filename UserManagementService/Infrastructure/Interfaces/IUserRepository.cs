using UserManagementService.Infrastructure.Entities;

namespace UserManagementService.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task AddEntityAsync(User entity);
        Task<User?> GetByUserNameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task<(IEnumerable<User> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
        Task UpdateAsync(User entity);
        Task SaveChangesAsync();

        Task AddRefreshTokenAsync(RefreshToken token);
        Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash);
        Task RevokeRefreshTokenAsync(RefreshToken token, string? replacedByHash = null);
    }
}
