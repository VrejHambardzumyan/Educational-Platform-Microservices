namespace UserManagementService.Application.Models.DTOs
{
    public class RegisterResponseDto
    {
        public bool RequiresOtp { get; set; }
        public string? Email { get; set; }
        public AuthResponseDto? Auth { get; set; }
    }
}
