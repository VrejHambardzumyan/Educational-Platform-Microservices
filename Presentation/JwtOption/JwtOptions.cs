namespace UserManagementService.Presentation.JwtOption
{
    public class JwtOptions
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? PrivateKeyPath { get; set; }
        public string? PublicKeyPath { get; set; }
    }
}
