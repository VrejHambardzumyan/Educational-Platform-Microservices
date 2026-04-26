using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseCatalogService.Migrations
{
    /// <inheritdoc />
    public partial class DropLessonVideoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl",
                schema: "course_catalog",
                table: "Lessons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                schema: "course_catalog",
                table: "Lessons",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
