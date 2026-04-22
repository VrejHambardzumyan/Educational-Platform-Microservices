namespace UserManagementService.Application.Authorization
{
    /// <summary>Compile-time constants for every permission name stored in the database.</summary>
    public static class Permissions
    {
        public const string UsersRead       = "users.read";
        public const string UsersReadAny    = "users.read_any";
        public const string UsersUpdateOwn  = "users.update_own";
        public const string UsersManage     = "users.manage";

        public const string CoursesRead     = "courses.read";
        public const string CoursesCreate   = "courses.create";
        public const string CoursesManage   = "courses.manage";

        public const string ContentUpload   = "content.upload";

        public const string EnrollmentsRead   = "enrollments.read";
        public const string EnrollmentsManage = "enrollments.manage";
    }
}
