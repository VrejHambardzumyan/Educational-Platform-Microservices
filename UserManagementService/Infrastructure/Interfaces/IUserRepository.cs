using UserManagementService.Infrastructure.Entities;

namespace UserManagementService.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task AddEntityAsync(User entity);
        Task<User?> GetByUserNameAsync(string username);
    }
}
