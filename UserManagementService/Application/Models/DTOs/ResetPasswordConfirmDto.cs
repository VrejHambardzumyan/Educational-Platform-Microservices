namespace UserManagementService.Application.Models.DTOs
{
    public class ResetPasswordConfirmDto
    {
        public required string Email { get; set; }
        public required string Otp { get; set; }
        public required string NewPassword { get; set; }
    }
}
