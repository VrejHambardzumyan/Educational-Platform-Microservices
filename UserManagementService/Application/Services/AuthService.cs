using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.DTOs;
using UserManagementService.Infrastructure.Configuration;
using UserManagementService.Infrastructure.Entities;
using UserManagementService.Infrastructure.Interfaces;

namespace UserManagementService.Application.Services
{
    public class AuthService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IOtpService otpService,
        IOptions<TwoFactorSettings> twoFactorOptions) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IOtpService _otpService = otpService;
        private readonly TwoFactorSettings _twoFactor = twoFactorOptions.Value;

        public async Task<RegisterResponseDto> RegisterUserAsync(string userName, string password, string email, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userRepository.GetByUserNameAsync(userName);
            if (existingUser != null)
                throw new InvalidOperationException($"User with username '{userName}' already exists.");

            var existingEmail = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (existingEmail != null)
                throw new InvalidOperationException("An account with this email already exists.");

            var user = new User
            {
                UserName = userName,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Email = email,
                IsEmailVerified = !_twoFactor.Enabled
            };

            await _userRepository.AddEntityAsync(user);

            if (_twoFactor.Enabled)
            {
                try
                {
                    await _otpService.GenerateAndSendAsync(user.Id, email, OtpPurpose.SignUp, cancellationToken);
                }
                catch 
                {
                    await _userRepository.DeleteAsync(user);
                    throw;
                }
                return new RegisterResponseDto { RequiresOtp = true, Email = email };
            }

            var (accessToken, refreshToken) = await IssueTokensAsync(user);
            return new RegisterResponseDto
            {
                RequiresOtp = false,
                Auth = new AuthResponseDto { AccessToken = accessToken, RefreshToken = refreshToken, UserId = user.Id }
            };
        }

        public async Task<AuthResponseDto> LoginUserAsync(string userName, string password)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (_twoFactor.Enabled && !user.IsEmailVerified)
                throw new UnauthorizedAccessException("Email not verified. Please complete signup OTP verification.");

            var (accessToken, refreshToken) = await IssueTokensAsync(user);
            return new AuthResponseDto { AccessToken = accessToken, RefreshToken = refreshToken, UserId = user.Id };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var tokenHash = HashToken(refreshToken);
            var storedToken = await _userRepository.GetRefreshTokenAsync(tokenHash);

            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var user = storedToken.User;
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newTokenHash = HashToken(newRefreshToken);

            await _userRepository.RevokeRefreshTokenAsync(storedToken, newTokenHash);
            await StoreRefreshTokenAsync(user.Id, newRefreshToken);

            return new AuthResponseDto { AccessToken = newAccessToken, RefreshToken = newRefreshToken, UserId = user.Id };
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var tokenHash = HashToken(refreshToken);
            var storedToken = await _userRepository.GetRefreshTokenAsync(tokenHash);
            if (storedToken != null)
                await _userRepository.RevokeRefreshTokenAsync(storedToken);
        }

        public async Task RequestOtpAsync(string email, string purpose, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken)
                ?? throw new KeyNotFoundException("No account found with this email.");

            await _otpService.GenerateAndSendAsync(user.Id, email, purpose, cancellationToken);
        }

        public async Task<AuthResponseDto> VerifySignupOtpAsync(string email, string otp, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken)
                ?? throw new KeyNotFoundException("No account found with this email.");

            var valid = await _otpService.VerifyAsync(user.Id, otp, OtpPurpose.SignUp, cancellationToken);
            if (!valid)
                throw new UnauthorizedAccessException("Invalid or expired OTP.");

            user.IsEmailVerified = true;
            await _userRepository.UpdateAsync(user);

            var (accessToken, refreshToken) = await IssueTokensAsync(user);
            return new AuthResponseDto { AccessToken = accessToken, RefreshToken = refreshToken, UserId = user.Id };
        }

        public async Task<bool> VerifyPaymentOtpAsync(string email, string otp, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken)
                ?? throw new KeyNotFoundException("No account found with this email.");

            return await _otpService.VerifyAsync(user.Id, otp, OtpPurpose.Payment, cancellationToken);
        }

        public async Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user == null) return; // don't reveal whether email exists

            await _otpService.GenerateAndSendAsync(user.Id, email, OtpPurpose.PasswordReset, cancellationToken);
        }

        public async Task ConfirmPasswordResetAsync(string email, string otp, string newPassword, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken)
                ?? throw new KeyNotFoundException("No account found with this email.");

            var valid = await _otpService.VerifyAsync(user.Id, otp, OtpPurpose.PasswordReset, cancellationToken);
            if (!valid)
                throw new UnauthorizedAccessException("Invalid or expired OTP.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
        }

        private async Task<(string accessToken, string refreshToken)> IssueTokensAsync(User user)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            await StoreRefreshTokenAsync(user.Id, refreshToken);
            return (accessToken, refreshToken);
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
