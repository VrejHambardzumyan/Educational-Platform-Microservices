namespace UserManagementService.Infrastructure.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string UserName { get; set; }

        public string? Email { get; set; }
        public required string Password { get; set; }

    }
}
