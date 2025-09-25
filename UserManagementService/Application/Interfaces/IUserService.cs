using UserManagementService.Application.Models.DTOs;
using UserManagementService.Infrastructure;

namespace UserManagementService.Application.Interfaces
{
    public interface IUserService
    {
        Task AddUser(UserDTO user);
    }
}
