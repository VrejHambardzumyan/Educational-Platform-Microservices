using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressTrackingService.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressTrachkingScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "progress_tracking");

            migrationBuilder.RenameTable(
                name: "LessonProgress",
                newName: "LessonProgress",
                newSchema: "progress_tracking");

            migrationBuilder.RenameTable(
                name: "CourseProgress",
                newName: "CourseProgress",
                newSchema: "progress_tracking");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "LessonProgress",
                schema: "progress_tracking",
                newName: "LessonProgress");

            migrationBuilder.RenameTable(
                name: "CourseProgress",
                schema: "progress_tracking",
                newName: "CourseProgress");
        }
    }
}
