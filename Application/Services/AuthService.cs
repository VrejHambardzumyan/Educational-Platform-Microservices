using Azure.Core;
using Microsoft.AspNetCore.Identity.Data;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.DTOs;
using UserManagementService.Infrastructure;
using UserManagementService.Infrastructure.Entities;
using UserManagementService.Infrastructure.Interfaces;
namespace UserManagementService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDTO> RegisterUser(string userName, string password)
        {
            var existingUser = await _userRepository.GetByUserNameAsync(userName);
            if (existingUser != null)
                throw new InvalidOperationException($"User with username '{userName}' already exists");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                UserName = userName,
                Password = hashedPassword
            };

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _userRepository.AddEntity(user);
           
            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponseDTO> LoginUser(string userName, string password)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new UnauthorizedAccessException("Invalid username or password");

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new UnauthorizedAccessException("Invalid username or password");

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

    }
}