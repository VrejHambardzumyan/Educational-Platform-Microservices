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
        Task DeleteAsync(User entity);

        Task AddRefreshTokenAsync(RefreshToken token);
        Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash);
        Task RevokeRefreshTokenAsync(RefreshToken token, string? replacedByHash = null);

        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task AddOtpAsync(OtpRecord otp, CancellationToken cancellationToken = default);
        Task<OtpRecord?> GetValidOtpAsync(int userId, string purpose, CancellationToken cancellationToken = default);
        Task MarkOtpUsedAsync(OtpRecord otp, CancellationToken cancellationToken = default);
    }
}
