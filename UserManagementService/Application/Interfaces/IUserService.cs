using Shared.Models;
using UserManagementService.Application.Models.DTOs;

namespace UserManagementService.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileResponseDto?> GetByIdAsync(int id);
        Task<UserProfileResponseDto?> UpdateProfileAsync(int userId, UpdateProfileRequestDto dto);
        Task<PagedResponse<UserProfileResponseDto>> GetAllAsync(int page, int pageSize);
        Task<bool> UpdateRoleAsync(int userId, string role);
        Task<bool> SoftDeleteAsync(int userId);
        Task<bool> SetOtpEnabledAsync(int userId, bool enabled);
        Task<bool> SwitchOwnRoleAsync(int userId, string role);
    }
}
