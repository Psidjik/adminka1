using CabinetBooking.Domain.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CabinetBooking.Domain.Data;

public class CabinetBookingDbContext : DbContext
{
    public static readonly string DefaultSchema = "cabinet_booking";
    
    public CabinetBookingDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Cabinet> Cabinets { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Lesson> Lessons { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema(DefaultSchema)
            .ApplyConfiguration(new CabinetConfiguration())
            .ApplyConfiguration(new BookingConfiguration())
            .ApplyConfiguration(new LessonConfiguration());
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseSnakeCaseNamingConvention();
        if (!optionsBuilder.IsConfigured)
            throw new InvalidOperationException("Context was not configured");
        base.OnConfiguring(optionsBuilder);
    }
}