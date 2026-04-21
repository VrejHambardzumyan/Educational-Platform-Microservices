using System.Security.Cryptography;
using System.Text;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.DTOs;
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

        public async Task<AuthResponseDto> RegisterUserAsync(string userName, string password, string email)
        {
            var existingUser = await _userRepository.GetByUserNameAsync(userName);
            if (existingUser != null)
                throw new InvalidOperationException($"User with username '{userName}' already exists");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                UserName = userName,
                Password = hashedPassword,
                Email = email
            };

            await _userRepository.AddEntityAsync(user);

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await StoreRefreshTokenAsync(user.Id, refreshToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id
            };
        }

        public async Task<AuthResponseDto> LoginUserAsync(string userName, string password)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new UnauthorizedAccessException("Invalid username or password");

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await StoreRefreshTokenAsync(user.Id, refreshToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var tokenHash = HashToken(refreshToken);
            var storedToken = await _userRepository.GetRefreshTokenAsync(tokenHash);

            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token");

            var user = storedToken.User;

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newTokenHash = HashToken(newRefreshToken);

            await _userRepository.RevokeRefreshTokenAsync(storedToken, newTokenHash);
            await StoreRefreshTokenAsync(user.Id, newRefreshToken);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                UserId = user.Id
            };
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var tokenHash = HashToken(refreshToken);
            var storedToken = await _userRepository.GetRefreshTokenAsync(tokenHash);

            if (storedToken != null)
                await _userRepository.RevokeRefreshTokenAsync(storedToken);
        }

        private async Task StoreRefreshTokenAsync(int userId, string refreshToken)
        {
            var token = new RefreshToken
            {
                UserId = userId,
                TokenHash = HashToken(refreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.AddRefreshTokenAsync(token);
        }

        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }
    }
}
