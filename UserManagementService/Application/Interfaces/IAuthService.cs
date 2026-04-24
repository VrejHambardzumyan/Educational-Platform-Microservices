using UserManagementService.Application.Models.DTOs;

namespace UserManagementService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterUserAsync(string userName, string password, string email, string role, CancellationToken cancellationToken = default);
        Task<AuthResponseDto> LoginUserAsync(string userName, string password);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);

        Task RequestOtpAsync(string email, string purpose, CancellationToken cancellationToken = default);
        Task<AuthResponseDto> VerifySignupOtpAsync(string email, string otp, CancellationToken cancellationToken = default);
        Task<bool> VerifyPaymentOtpAsync(string email, string otp, CancellationToken cancellationToken = default);
        Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default);
        Task ConfirmPasswordResetAsync(string email, string otp, string newPassword, CancellationToken cancellationToken = default);
    }
}
