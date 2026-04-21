namespace UserManagementService.Infrastructure.Entities
{
    public class OtpRecord
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string OtpCodeHash { get; set; }
        public required string Purpose { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }

    public static class OtpPurpose
    {
        public const string SignUp = "SignUp";
        public const string PasswordReset = "PasswordReset";
        public const string Payment = "Payment";
    }
}
