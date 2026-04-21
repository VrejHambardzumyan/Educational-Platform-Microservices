using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using UserManagementService.Application.Interfaces;
using UserManagementService.Infrastructure.Configuration;
using UserManagementService.Infrastructure.Entities;
using UserManagementService.Infrastructure.Interfaces;

namespace UserManagementService.Application.Services
{
    public class OtpService(
        IUserRepository userRepo,
        IEmailService emailService,
        IOptions<TwoFactorSettings> twoFactorOptions) : IOtpService
    {
        private readonly IUserRepository _userRepo = userRepo;
        private readonly IEmailService _emailService = emailService;
        private readonly TwoFactorSettings _settings = twoFactorOptions.Value;

        public async Task GenerateAndSendAsync(int userId, string email, string purpose, CancellationToken cancellationToken = default)
        {
            var code = GenerateCode();
            var record = new OtpRecord
            {
                UserId = userId,
                OtpCodeHash = HashCode(code),
                Purpose = purpose,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_settings.OtpExpiryMinutes)
            };

            await _userRepo.AddOtpAsync(record, cancellationToken);
            await _emailService.SendOtpAsync(email, code, purpose, cancellationToken);
        }

        public async Task<bool> VerifyAsync(int userId, string otpCode, string purpose, CancellationToken cancellationToken = default)
        {
            var record = await _userRepo.GetValidOtpAsync(userId, purpose, cancellationToken);
            if (record == null) return false;
            if (record.OtpCodeHash != HashCode(otpCode)) return false;

            await _userRepo.MarkOtpUsedAsync(record, cancellationToken);
            return true;
        }

        private static string GenerateCode()
        {
            var bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            var number = BitConverter.ToUInt32(bytes, 0) % 1_000_000;
            return number.ToString("D6");
        }

        private static string HashCode(string code)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code));
            return Convert.ToBase64String(bytes);
        }
    }
}
