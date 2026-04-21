using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Application.Models.DTOs
{
    public class VerifyOtpDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be exactly 6 digits.")]
        public required string Otp { get; set; }

        /// <summary>
        /// Allowed values: <c>SignUp</c>, <c>Payment</c>.
        /// Use <c>POST /auth/reset-password/confirm</c> for password-reset OTP verification.
        /// </summary>
        [Required]
        [RegularExpression(@"^(SignUp|Payment)$",
            ErrorMessage = "Purpose must be 'SignUp' or 'Payment'. Use /auth/reset-password/confirm for password resets.")]
        public required string Purpose { get; set; }
    }
}
