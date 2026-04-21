using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Application.Models.DTOs
{
    public class ResetPasswordConfirmDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be exactly 6 digits.")]
        public required string Otp { get; set; }

        [Required]
        public required string NewPassword { get; set; }
    }
}
