namespace UserManagementService.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task AddEntity(User entity);
    }
}
