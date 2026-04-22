using Microsoft.EntityFrameworkCore;
using UserManagementService.Infrastructure.Entities;
using UserManagementService.Infrastructure.Interfaces;

namespace UserManagementService.Infrastructure.Repositories
{
    public class RoleRepository(UserDbContext dbContext) : IRoleRepository
    {
        private readonly UserDbContext _context = dbContext;

        /// <summary>Finds a role by its name (case-sensitive). Returns null when not found.</summary>
        public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
            await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
}
