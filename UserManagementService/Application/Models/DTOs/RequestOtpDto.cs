using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Application.Models.DTOs
{
    public class RequestOtpDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        /// <summary>
        /// Allowed values: <c>SignUp</c>, <c>Payment</c>.
        /// Use <c>POST /auth/reset-password/request</c> for password-reset OTPs.
        /// </summary>
        [Required]
        [RegularExpression(@"^(SignUp|Payment)$",
            ErrorMessage = "Purpose must be 'SignUp' or 'Payment'. Use /auth/reset-password/request for password resets.")]
        public required string Purpose { get; set; }
    }
}
