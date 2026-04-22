using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create lookup tables before touching the users table
            migrationBuilder.CreateTable(
                name: "permissions",
                schema: "user_management",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "user_management",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            // 2. Seed lookup data so the FK migration below can reference valid ids
            migrationBuilder.InsertData(
                schema: "user_management",
                table: "permissions",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1,  "Read own profile",                              "users.read" },
                    { 2,  "Read any user's profile",                       "users.read_any" },
                    { 3,  "Update own profile",                            "users.update_own" },
                    { 4,  "List, delete, and reassign roles for any user", "users.manage" },
                    { 5,  "Browse the course catalog",                     "courses.read" },
                    { 6,  "Create new courses",                            "courses.create" },
                    { 7,  "Edit or delete any course",                     "courses.manage" },
                    { 8,  "Upload lesson and section content",             "content.upload" },
                    { 9,  "View own enrollments",                          "enrollments.read" },
                    { 10, "Manage all enrollments",                        "enrollments.manage" }
                });

            migrationBuilder.InsertData(
                schema: "user_management",
                table: "roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Enrolled learner",         "Student" },
                    { 2, "Course creator",           "Instructor" },
                    { 3, "Platform administrator",   "Admin" }
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                schema: "user_management",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "user_management",
                        principalTable: "permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "user_management",
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "user_management",
                table: "role_permissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1, 1 }, { 3, 1 }, { 5, 1 }, { 9, 1 },
                    { 1, 2 }, { 3, 2 }, { 5, 2 }, { 6, 2 }, { 8, 2 }, { 9, 2 },
                    { 1, 3 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 3 },
                    { 6, 3 }, { 7, 3 }, { 8, 3 }, { 9, 3 }, { 10, 3 }
                });

            // 3. Add RoleId as nullable so existing rows aren't blocked
            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                schema: "user_management",
                table: "users",
                type: "integer",
                nullable: true);

            // 4. Migrate old Role string values to the new FK — preserves existing role assignments
            migrationBuilder.Sql(@"
                UPDATE user_management.users
                SET ""RoleId"" = CASE ""Role""
                    WHEN 'Instructor' THEN 2
                    WHEN 'Admin'      THEN 3
                    ELSE                   1
                END;
            ");

            // 5. Make the column NOT NULL with a default of Student (1) for any future inserts
            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                schema: "user_management",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            // 6. Now safe to drop the old string column
            migrationBuilder.DropColumn(
                name: "Role",
                schema: "user_management",
                table: "users");

            // 7. Indexes
            migrationBuilder.CreateIndex(
                name: "IX_users_RoleId",
                schema: "user_management",
                table: "users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_Name",
                schema: "user_management",
                table: "permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_PermissionId",
                schema: "user_management",
                table: "role_permissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_Name",
                schema: "user_management",
                table: "roles",
                column: "Name",
                unique: true);

            // 8. FK from users → roles
            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_RoleId",
                schema: "user_management",
                table: "users",
                column: "RoleId",
                principalSchema: "user_management",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_RoleId",
                schema: "user_management",
                table: "users");

            migrationBuilder.DropTable(
                name: "role_permissions",
                schema: "user_management");

            migrationBuilder.DropTable(
                name: "permissions",
                schema: "user_management");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "user_management");

            migrationBuilder.DropIndex(
                name: "IX_users_RoleId",
                schema: "user_management",
                table: "users");

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "user_management",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                schema: "user_management",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "Student");
        }
    }
}
