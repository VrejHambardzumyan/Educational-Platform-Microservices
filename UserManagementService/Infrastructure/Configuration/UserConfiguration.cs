using Microsoft.EntityFrameworkCore;
using UserManagementService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserManagementService.Infrastructure.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users"); 
            
            builder.HasKey(u => u.Id);
            
            builder.Property(u => u.UserName)
                .HasColumnName("UserName")
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(u => u.Password)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasColumnName("Email address")
                .IsRequired();

            builder.HasIndex(u => u.UserName)
                    .IsUnique();
        }
    }
}
