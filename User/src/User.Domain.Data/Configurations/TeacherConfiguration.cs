using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Domain.ValueObject;

namespace User.Domain.Data.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("teachers");
        
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .ValueGeneratedNever();
        
        builder.Property(t => t.PersonalNumber)
            .IsRequired();
        
        builder.Property(t => t.Password)
            .IsRequired();
        
        builder.Property(t => t.FullName)
            .HasColumnName("full_name")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null), 
                v => JsonSerializer.Deserialize<FullName>(v, (JsonSerializerOptions)null)
            );
        builder.HasMany(ua => ua.RefreshTokens)
            .WithOne(rt => rt.Teacher)
            .HasForeignKey(rt => rt.TeacherId);
    }
}