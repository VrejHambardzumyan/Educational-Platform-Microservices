using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseEnrollment.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseEnrollmentSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "course_enrollment");

            migrationBuilder.RenameTable(
                name: "Enrollment",
                newName: "Enrollment",
                newSchema: "course_enrollment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Enrollment",
                schema: "course_enrollment",
                newName: "Enrollment");
        }
    }
}
