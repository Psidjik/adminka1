using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CabinetBooking.Domain.Data.Configurations;

public class CabinetConfiguration : IEntityTypeConfiguration<Cabinet>
{
    public void Configure(EntityTypeBuilder<Cabinet> builder)
    {
        builder.ToTable("cabinets");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.CabinetType).HasConversion<string>();
        builder.Property(c => c.Id).HasColumnName("Number");
        
        builder
            .HasMany(c => c.Bookings)
            .WithOne()
            .HasForeignKey(b => b.CabinetId)
            .IsRequired();
    }
}