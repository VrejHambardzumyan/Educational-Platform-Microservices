using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.DTOs;
using UserManagementService.Infrastructure.Entities;
using UserManagementService.Infrastructure.Interfaces;

namespace UserManagementService.Application.Services
{
    public class UserService(IUserRepository userRepo) : IUserService
    {
        private readonly IUserRepository _userRepo = userRepo;

        public async Task AddUserAsync(UserDto userModel)
        {
            User user = new()
            {
                UserName = userModel.Name,
                Password = userModel.Password,
                Email = userModel.Email,
            };
            await _userRepo.AddEntityAsync(user);
        }
    }
}
