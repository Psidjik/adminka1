using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CabinetBooking.Domain.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("bookings");
        
        builder.HasKey(b => b.Id);
        
        builder.HasOne(b => b.Lesson)
            .WithMany()
            .HasForeignKey(b => b.LessonId)
            .IsRequired();
    }
}