using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagementService.Infrastructure.Entities;
using static UserManagementService.Infrastructure.Entities.RoleIds;

namespace UserManagementService.Infrastructure.Configuration
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("role_permissions");
            builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            builder.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student: read own, update own, browse courses, view enrollments
            // Instructor: + create courses, upload content
            // Admin: all permissions
            builder.HasData(
                new RolePermission { RoleId = Student, PermissionId = 1  },
                new RolePermission { RoleId = Student, PermissionId = 3  },
                new RolePermission { RoleId = Student, PermissionId = 5  },
                new RolePermission { RoleId = Student, PermissionId = 9  },

                new RolePermission { RoleId = Instructor, PermissionId = 1  },
                new RolePermission { RoleId = Instructor, PermissionId = 3  },
                new RolePermission { RoleId = Instructor, PermissionId = 5  },
                new RolePermission { RoleId = Instructor, PermissionId = 6  },
                new RolePermission { RoleId = Instructor, PermissionId = 8  },
                new RolePermission { RoleId = Instructor, PermissionId = 9  },

                new RolePermission { RoleId = Admin, PermissionId = 1  },
                new RolePermission { RoleId = Admin, PermissionId = 2  },
                new RolePermission { RoleId = Admin, PermissionId = 3  },
                new RolePermission { RoleId = Admin, PermissionId = 4  },
                new RolePermission { RoleId = Admin, PermissionId = 5  },
                new RolePermission { RoleId = Admin, PermissionId = 6  },
                new RolePermission { RoleId = Admin, PermissionId = 7  },
                new RolePermission { RoleId = Admin, PermissionId = 8  },
                new RolePermission { RoleId = Admin, PermissionId = 9  },
                new RolePermission { RoleId = Admin, PermissionId = 10 }
            );
        }
    }
}
