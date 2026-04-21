using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseCatalogService.Migrations
{
    /// <inheritdoc />
    public partial class updatedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_courses_Id",
                table: "courses");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "courses",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "courses",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "courses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "courses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<int>(
                name: "InstructorId",
                table: "courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "courses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "courses",
                type: "text",
                nullable: false,
                defaultValue: "Draft");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "courses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "courses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_courses_Category",
                table: "courses",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_courses_InstructorId",
                table: "courses",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_courses_Status",
                table: "courses",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_courses_Category",
                table: "courses");

            migrationBuilder.DropIndex(
                name: "IX_courses_InstructorId",
                table: "courses");

            migrationBuilder.DropIndex(
                name: "IX_courses_Status",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "courses");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "courses",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "courses",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.CreateIndex(
                name: "IX_courses_Id",
                table: "courses",
                column: "Id",
                unique: true);
        }
    }
}
