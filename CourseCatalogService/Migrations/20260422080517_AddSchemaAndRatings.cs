using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CourseCatalogService.Migrations
{
    /// <inheritdoc />
    public partial class AddSchemaAndRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "course_catalog");

            migrationBuilder.RenameTable(
                name: "sections",
                newName: "sections",
                newSchema: "course_catalog");

            migrationBuilder.RenameTable(
                name: "Lessons",
                newName: "Lessons",
                newSchema: "course_catalog");

            migrationBuilder.RenameTable(
                name: "courses",
                newName: "courses",
                newSchema: "course_catalog");

            migrationBuilder.RenameTable(
                name: "content_blocks",
                newName: "content_blocks",
                newSchema: "course_catalog");

            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                schema: "course_catalog",
                table: "courses",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "RatingCount",
                schema: "course_catalog",
                table: "courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "course_ratings",
                schema: "course_catalog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Feedback = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_course_ratings_courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "course_catalog",
                        principalTable: "courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_ratings_CourseId_UserId",
                schema: "course_catalog",
                table: "course_ratings",
                columns: new[] { "CourseId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_ratings",
                schema: "course_catalog");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                schema: "course_catalog",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "RatingCount",
                schema: "course_catalog",
                table: "courses");

            migrationBuilder.RenameTable(
                name: "sections",
                schema: "course_catalog",
                newName: "sections");

            migrationBuilder.RenameTable(
                name: "Lessons",
                schema: "course_catalog",
                newName: "Lessons");

            migrationBuilder.RenameTable(
                name: "courses",
                schema: "course_catalog",
                newName: "courses");

            migrationBuilder.RenameTable(
                name: "content_blocks",
                schema: "course_catalog",
                newName: "content_blocks");
        }
    }
}
