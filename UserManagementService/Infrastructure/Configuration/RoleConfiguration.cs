using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagementService.Infrastructure.Entities;

namespace UserManagementService.Infrastructure.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Description)
                .HasMaxLength(200);

            builder.HasIndex(r => r.Name).IsUnique();

            builder.HasData(
                new Role { Id = 1, Name = "Student",    Description = "Enrolled learner" },
                new Role { Id = 2, Name = "Instructor", Description = "Course creator" },
                new Role { Id = 3, Name = "Admin",      Description = "Platform administrator" }
            );
        }
    }
}
