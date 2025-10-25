using Microsoft.EntityFrameworkCore;
using CorisSeguros.Ingestion.Models;

namespace CorisSeguros.Ingestion.Infrastructure;

public class IngestionDbContext : DbContext
{
    public IngestionDbContext(DbContextOptions<IngestionDbContext> options) : base(options)
    {
    }

    public DbSet<Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BlobUrl).IsRequired();
            entity.Property(e => e.AnalystId).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>();
            
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.AnalystId);
            entity.HasIndex(e => e.UploadedAt);
        });
    }
}

