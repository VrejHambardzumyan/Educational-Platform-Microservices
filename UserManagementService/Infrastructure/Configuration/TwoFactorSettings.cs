namespace UserManagementService.Infrastructure.Configuration
{
    public class TwoFactorSettings
    {
        public bool Enabled { get; set; } = false;
        public int OtpExpiryMinutes { get; set; } = 10;
    }
}
