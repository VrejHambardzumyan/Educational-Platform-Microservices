using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagementService.Infrastructure.Entities;

namespace UserManagementService.Infrastructure.Configuration
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("permissions");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(200);

            builder.HasIndex(p => p.Name).IsUnique();

            builder.HasData(
                new Permission { Id = 1,  Name = "users.read",          Description = "Read own profile" },
                new Permission { Id = 2,  Name = "users.read_any",      Description = "Read any user's profile" },
                new Permission { Id = 3,  Name = "users.update_own",    Description = "Update own profile" },
                new Permission { Id = 4,  Name = "users.manage",        Description = "List, delete, and reassign roles for any user" },
                new Permission { Id = 5,  Name = "courses.read",        Description = "Browse the course catalog" },
                new Permission { Id = 6,  Name = "courses.create",      Description = "Create new courses" },
                new Permission { Id = 7,  Name = "courses.manage",      Description = "Edit or delete any course" },
                new Permission { Id = 8,  Name = "content.upload",      Description = "Upload lesson and section content" },
                new Permission { Id = 9,  Name = "enrollments.read",    Description = "View own enrollments" },
                new Permission { Id = 10, Name = "enrollments.manage",  Description = "Manage all enrollments" }
            );
        }
    }
}
