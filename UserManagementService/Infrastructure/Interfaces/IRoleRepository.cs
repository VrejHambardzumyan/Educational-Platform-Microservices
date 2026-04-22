using UserManagementService.Infrastructure.Entities;

namespace UserManagementService.Infrastructure.Interfaces
{
    public interface IRoleRepository
    {
        /// <summary>Finds a role by its name (case-sensitive). Returns null when not found.</summary>
        Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
