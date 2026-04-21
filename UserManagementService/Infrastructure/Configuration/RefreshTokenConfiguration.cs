using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagementService.Infrastructure.Entities;

namespace UserManagementService.Infrastructure.Configuration
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.TokenHash)
                .IsRequired();

            builder.Property(r => r.ExpiresAt)
                .IsRequired();

            builder.Property(r => r.IsRevoked)
                .HasDefaultValue(false);

            builder.HasIndex(r => r.TokenHash);
            builder.HasIndex(r => r.UserId);

            builder.HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
