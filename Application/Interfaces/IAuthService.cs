using UserManagementService.Application.Models.DTOs;

namespace UserManagementService.Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterUser(string userName, string password);
        Task<AuthResponseDTO> LoginUser(string userName, string password);
    }
}
