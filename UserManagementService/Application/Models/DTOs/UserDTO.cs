namespace UserManagementService.Application.Models.DTOs
{
    public class UserDto
    {
        public required string Name { get; init; }
        public  required string Email { get; init; }
        public required string Password { get; init; }
    }
}
