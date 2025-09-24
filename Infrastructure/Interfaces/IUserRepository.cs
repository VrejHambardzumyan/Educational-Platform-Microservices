namespace UserManagementService.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task AddEntity(User entity);
        Task<User?> GetByUserNameAsync(string username);

    }
}
