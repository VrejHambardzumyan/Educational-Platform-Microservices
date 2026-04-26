using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using UserManagementService.Application.Interfaces;
using UserManagementService.Infrastructure.Configuration;

namespace UserManagementService.Application.Services
{
    public class EmailService(IOptions<SmtpSettings> smtpOptions, ILogger<EmailService> logger) : IEmailService
    {
        private readonly SmtpSettings _settings = smtpOptions.Value;
        private readonly ILogger<EmailService> _logger = logger;

        public async Task SendOtpAsync(string toEmail, string otp, string purpose, CancellationToken cancellationToken = default)
        {
            var subject = purpose switch
            {
                "SignUp" => "Verify your email",
                "PasswordReset" => "Reset your password",
                "Payment" => "Confirm your payment",
                _ => "Your verification code"
            };

            var body = $"""
                <h2>{subject}</h2>
                <p>Your verification code is:</p>
                <h1 style="letter-spacing:8px">{otp}</h1>
                <p>This code expires in {GetExpiryText(purpose)} minutes. Do not share it with anyone.</p>
                """;

            if (!_settings.IsConfigured)
            {
                _logger.LogInformation("[2FA] OTP for {Email} ({Purpose}): {Otp}", toEmail, purpose, otp);
                return;
            }

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.UseSsl,
                Credentials = new NetworkCredential(_settings.UserName, _settings.Password)
            };

            var message = new MailMessage
            {
                From = new MailAddress(_settings.FromAddress, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message, cancellationToken);
        }

        private static int GetExpiryText(string purpose) => purpose switch
        {
            "Payment" => 5,
            "SignUp" => 1,
            _ => 10
        };
    }
}
