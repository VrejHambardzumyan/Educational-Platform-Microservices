using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models;
using UserManagementService.Infrastructure;
using UserManagementService.Infrastructure.Interfaces;
using UserManagementService.Infrastructure.Repositories;

namespace UserManagementService.Application.Services
{
    public class UserService(IUserRepository userRepo) : IUserService
    {
        private readonly IUserRepository _userRepo = userRepo;

        public async Task AddUser(UserModel userModel)
        {
            User user = new User() { Name = userModel.Name };
            await _userRepo.AddEntity(user);
        }
    }
}
