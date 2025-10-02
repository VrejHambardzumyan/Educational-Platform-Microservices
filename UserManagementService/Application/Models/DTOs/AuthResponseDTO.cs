namespace UserManagementService.Application.Models.DTOs
{
    public class AuthResponseDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken  { get; set; }
    }
}
