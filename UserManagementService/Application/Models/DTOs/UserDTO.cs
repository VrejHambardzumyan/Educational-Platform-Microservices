namespace UserManagementService.Application.Models.DTOs
{
    public class UserDTO
    {
        public int Id { get; init; } 
        public required string Name { get; init; }
        public  string? Email { get; init; }
        public required string Password { get; init; }
    }
}
