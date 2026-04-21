namespace UserManagementService.Application.Interfaces
{
    public interface IOtpService
    {
        Task GenerateAndSendAsync(int userId, string email, string purpose, CancellationToken cancellationToken = default);
        Task<bool> VerifyAsync(int userId, string otpCode, string purpose, CancellationToken cancellationToken = default);
    }
}
