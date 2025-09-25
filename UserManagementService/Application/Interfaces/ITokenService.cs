using UserManagementService.Infrastructure;
using UserManagementService.Infrastructure.Entities;

namespace UserManagementService.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
