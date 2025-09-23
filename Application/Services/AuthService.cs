using Microsoft.AspNetCore.Identity.Data;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.DTOs;
using UserManagementService.Infrastructure;
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

        public async Task RegisterUser(string userName, string password)
        {
            var existingUser = await _userRepository.GetByUserNameAsync(userName);
            if (existingUser != null)
                throw new Exception("User already exists!");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                UserName = userName,
                Password = hashedPassword
            };

            await _userRepository.AddEntity(user);
        }

        public async Task<AuthResponseDTO> LoginUser(string userName, string password)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new Exception("Invalid password");
           
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