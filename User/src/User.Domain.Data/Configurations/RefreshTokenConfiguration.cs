using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User.Domain.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.HasOne(rt => rt.Teacher) 
            .WithMany(u => u.RefreshTokens) 
            .HasForeignKey(rt => rt.TeacherId);
    }
}
