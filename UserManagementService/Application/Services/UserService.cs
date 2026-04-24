using Shared.Models;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.DTOs;
using UserManagementService.Infrastructure.Interfaces;

namespace UserManagementService.Application.Services
{
    public class UserService(IUserRepository userRepo, IRoleRepository roleRepo) : IUserService
    {
        private readonly IUserRepository _userRepo = userRepo;
        private readonly IRoleRepository _roleRepo = roleRepo;

        private static readonly HashSet<string> ValidRoles = ["Student", "Instructor", "Admin"];
        private static readonly HashSet<string> SwitchableRoles = ["Student", "Instructor"];

        /// <summary>Returns a user's public profile, or null when the user does not exist.</summary>
        public async Task<UserProfileResponseDto?> GetByIdAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return null;

            return new UserProfileResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.UserRole?.Name ?? "Student",
                CreatedAt = user.CreatedAt
            };
        }

        /// <summary>Updates mutable profile fields for the given user. Returns null when the user does not exist.</summary>
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
                Role = user.UserRole?.Name ?? "Student",
                CreatedAt = user.CreatedAt
            };
        }

        /// <summary>Returns a paginated list of all non-deleted users.</summary>
        public async Task<PagedResponse<UserProfileResponseDto>> GetAllAsync(int page, int pageSize)
        {
            var (users, totalCount) = await _userRepo.GetAllAsync(page, pageSize);

            return new PagedResponse<UserProfileResponseDto>
            {
                Items = users.Select(u => new UserProfileResponseDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Role = u.UserRole?.Name ?? "Student",
                    CreatedAt = u.CreatedAt
                }),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Changes the role of the specified user. Throws <see cref="ArgumentException"/> for
        /// unknown role names. Returns false when the user does not exist.
        /// </summary>
        public async Task<bool> UpdateRoleAsync(int userId, string role)
        {
            if (!ValidRoles.Contains(role))
                throw new ArgumentException($"Invalid role. Valid roles: {string.Join(", ", ValidRoles)}");

            var roleEntity = await _roleRepo.GetByNameAsync(role)
                ?? throw new ArgumentException($"Role '{role}' not found in the database.");

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            user.RoleId = roleEntity.Id;
            await _userRepo.UpdateAsync(user);
            return true;
        }

        /// <summary>Soft-deletes the specified user. Returns false when the user does not exist.</summary>
        public async Task<bool> SoftDeleteAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            user.IsDeleted = true;
            await _userRepo.UpdateAsync(user);
            return true;
        }

        /// <summary>
        /// Lets a user switch their own role between Student and Instructor.
        /// Throws <see cref="ArgumentException"/> when <paramref name="role"/> is not in <see cref="SwitchableRoles"/>.
        /// </summary>
        public async Task<bool> SwitchOwnRoleAsync(int userId, string role)
        {
            if (!SwitchableRoles.Contains(role))
                throw new ArgumentException("You can only switch between 'Student' and 'Instructor'.");

            var roleEntity = await _roleRepo.GetByNameAsync(role)
                ?? throw new ArgumentException($"Role '{role}' not found in the database.");

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            user.RoleId = roleEntity.Id;
            await _userRepo.UpdateAsync(user);
            return true;
        }

        /// <summary>Flips OtpEnabled for the given user. Returns false when the user does not exist.</summary>
        public async Task<bool> SetOtpEnabledAsync(int userId, bool enabled)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            user.OtpEnabled = enabled;
            await _userRepo.UpdateAsync(user);
            return true;
        }
    }
}
