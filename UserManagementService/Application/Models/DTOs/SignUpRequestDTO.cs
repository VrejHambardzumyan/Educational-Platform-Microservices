namespace UserManagementService.Application.Models.DTOs
{
    public class SignUpRequestDTO
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
