namespace UserManagementService.Application.Models.DTOs
{
    public class AuthResponseDTO
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken  { get; set; }
    }
}
