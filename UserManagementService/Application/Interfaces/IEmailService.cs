namespace UserManagementService.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpAsync(string toEmail, string otp, string purpose, CancellationToken cancellationToken = default);
    }
}
