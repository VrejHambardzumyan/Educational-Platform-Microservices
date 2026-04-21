namespace UserManagementService.Application.Models.DTOs
{
    public class UserProfileResponseDto
    {
        public int Id { get; init; }
        public required string UserName { get; init; }
        public required string Email { get; init; }
        public required string Role { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
