namespace UserManagementService.Infrastructure.Entities
{
    /// <summary>
    /// Stable primary-key values for the seeded roles in the <c>roles</c> table.
    /// These match the <c>HasData</c> seed in <see cref="Configuration.RoleConfiguration"/>.
    /// </summary>
    public static class RoleIds
    {
        public const int Student    = 1;
        public const int Instructor = 2;
        public const int Admin      = 3;
    }
}
