namespace UserManagementService.Application.Models.DTOs
{
    public class SignInRequestDto
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
