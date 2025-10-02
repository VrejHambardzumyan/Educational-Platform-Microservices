namespace UserManagementService.Application.Models.DTOs
{
    public class SignUpRequestDto
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
    }
}
