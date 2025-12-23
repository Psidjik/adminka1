using Microsoft.EntityFrameworkCore;
using User.Domain;
using User.Domain.Data.Configurations;

namespace User.Domain.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public static readonly string DefaultSchema = "user";

    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema(DefaultSchema)
            .ApplyConfiguration(new TeacherConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
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