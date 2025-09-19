namespace UserManagementService.Application.Models.DTOs
{
    public class SignInRequestDTO
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
