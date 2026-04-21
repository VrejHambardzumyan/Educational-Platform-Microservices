namespace UserManagementService.Infrastructure.Configuration
{
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool UseSsl { get; set; } = true;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
        public string FromName { get; set; } = "Educational Platform";

        public bool IsConfigured =>
            !string.IsNullOrEmpty(Host) &&
            !string.IsNullOrEmpty(UserName) &&
            !string.IsNullOrEmpty(Password) &&
            !string.IsNullOrEmpty(FromAddress);
    }
}
