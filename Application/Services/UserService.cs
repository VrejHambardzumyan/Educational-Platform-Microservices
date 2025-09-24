using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.DTOs;
using UserManagementService.Infrastructure;
using UserManagementService.Infrastructure.Interfaces;
using UserManagementService.Infrastructure.Repositories;

namespace UserManagementService.Application.Services
{
    public class UserService(IUserRepository userRepo) : IUserService
    {
        private readonly IUserRepository _userRepo = userRepo;

        public async Task AddUser(UserDTO userModel)
        {
            User user = new() { 
                UserName = userModel.Name,
                Email = userModel.Email,
                Password = userModel.Password,
            };
            await _userRepo.AddEntity(user);
        }
    }
}
