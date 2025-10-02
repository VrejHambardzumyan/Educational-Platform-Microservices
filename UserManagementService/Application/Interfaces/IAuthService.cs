using UserManagementService.Application.Models.DTOs;

namespace UserManagementService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterUserAsync(string userName, string password, string email);
        Task<AuthResponseDto> LoginUserAsync(string userName, string password);
    }
}
