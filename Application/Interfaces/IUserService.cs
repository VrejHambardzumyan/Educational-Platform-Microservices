using UserManagementService.Application.Models;
using UserManagementService.Infrastructure;

namespace UserManagementService.Application.Interfaces
{
    public interface IUserService
    {
        Task AddUser(UserModel user);
    }
}
