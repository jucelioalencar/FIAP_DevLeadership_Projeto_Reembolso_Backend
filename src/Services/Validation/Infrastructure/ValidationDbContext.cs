using Microsoft.EntityFrameworkCore;
using CorisSeguros.Validation.Models;

namespace CorisSeguros.Validation.Infrastructure;

public class ValidationDbContext : DbContext
{
    public ValidationDbContext(DbContextOptions<ValidationDbContext> options) : base(options)
    {
    }

    public DbSet<ValidationRecord> ValidationRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ValidationRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FlightNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PassengerName).HasMaxLength(255);
            entity.Property(e => e.Status).HasConversion<string>();
            
            entity.HasIndex(e => e.FlightNumber);
            entity.HasIndex(e => e.ValidationTimestamp);
        });
    }
}



