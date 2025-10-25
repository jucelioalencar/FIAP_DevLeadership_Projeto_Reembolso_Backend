using Microsoft.EntityFrameworkCore;
using CorisSeguros.Notification.Models;

namespace CorisSeguros.Notification.Infrastructure;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<NotificationRecord> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NotificationRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Recipient).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.RecipientType).HasConversion<string>();
            entity.Property(e => e.NotificationType).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>();
            
            entity.HasIndex(e => e.DocumentId);
            entity.HasIndex(e => e.RecipientType);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}



