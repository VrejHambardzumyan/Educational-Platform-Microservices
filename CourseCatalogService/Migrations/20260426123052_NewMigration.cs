using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace CourseCatalogService.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "search_vector",
                schema: "course_catalog",
                table: "courses",
                type: "tsvector",
                nullable: false,
                computedColumnSql: "to_tsvector('english', coalesce(\"Title\",'') || ' ' || coalesce(\"Description\",''))",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_courses_search_vector",
                schema: "course_catalog",
                table: "courses",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_courses_search_vector",
                schema: "course_catalog",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "search_vector",
                schema: "course_catalog",
                table: "courses");
        }
    }
}
