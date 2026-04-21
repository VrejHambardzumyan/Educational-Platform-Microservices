using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Application.Models.DTOs
{
    public class ResetPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
