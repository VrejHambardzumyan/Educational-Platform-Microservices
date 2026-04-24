namespace UserManagementService.Infrastructure.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

        public int RoleId { get; set; } = RoleIds.Student;
        public Role UserRole { get; set; } = null!;

        public bool IsDeleted { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool OtpEnabled { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
