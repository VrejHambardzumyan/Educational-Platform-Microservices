using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AddUserManagementSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "user_management");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "users",
                newSchema: "user_management");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                newName: "refresh_tokens",
                newSchema: "user_management");

            migrationBuilder.RenameTable(
                name: "otp_records",
                newName: "otp_records",
                newSchema: "user_management");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "users",
                schema: "user_management",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                schema: "user_management",
                newName: "refresh_tokens");

            migrationBuilder.RenameTable(
                name: "otp_records",
                schema: "user_management",
                newName: "otp_records");
        }
    }
}
