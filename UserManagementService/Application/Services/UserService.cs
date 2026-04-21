using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.DTOs;
using UserManagementService.Infrastructure.Interfaces;

namespace UserManagementService.Application.Services
{
    public class UserService(IUserRepository userRepo) : IUserService
    {
        private readonly IUserRepository _userRepo = userRepo;

        private static readonly HashSet<string> ValidRoles = ["Student", "Instructor", "Admin"];

        public async Task<UserProfileResponseDto?> GetByIdAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return null;

            return new UserProfileResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserProfileResponseDto?> UpdateProfileAsync(int userId, UpdateProfileRequestDto dto)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            await _userRepo.UpdateAsync(user);

            return new UserProfileResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<PagedResponseDto<UserProfileResponseDto>> GetAllAsync(int page, int pageSize)
        {
            var (users, totalCount) = await _userRepo.GetAllAsync(page, pageSize);

            return new PagedResponseDto<UserProfileResponseDto>
            {
                Items = users.Select(u => new UserProfileResponseDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                }),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<bool> UpdateRoleAsync(int userId, string role)
        {
            if (!ValidRoles.Contains(role))
                throw new ArgumentException($"Invalid role. Valid roles: {string.Join(", ", ValidRoles)}");

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            user.Role = role;
            await _userRepo.UpdateAsync(user);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            user.IsDeleted = true;
            await _userRepo.UpdateAsync(user);
            return true;
        }
    }
}
