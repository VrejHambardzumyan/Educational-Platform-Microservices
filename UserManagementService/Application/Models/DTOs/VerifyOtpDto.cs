namespace UserManagementService.Application.Models.DTOs
{
    public class VerifyOtpDto
    {
        public required string Email { get; set; }
        public required string Otp { get; set; }
        public required string Purpose { get; set; }
    }
}
