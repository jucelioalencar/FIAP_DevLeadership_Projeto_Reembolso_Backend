using Microsoft.EntityFrameworkCore;
using CorisSeguros.Analysis.Models;

namespace CorisSeguros.Analysis.Infrastructure;

public class AnalysisDbContext : DbContext
{
    public AnalysisDbContext(DbContextOptions<AnalysisDbContext> options) : base(options)
    {
    }

    public DbSet<BusinessRule> BusinessRules { get; set; }
    public DbSet<AnalysisRecord> AnalysisRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BusinessRule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Condition).IsRequired();
            entity.Property(e => e.Action).IsRequired();
            
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.Priority);
        });

        modelBuilder.Entity<AnalysisRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Recommendation).IsRequired().HasMaxLength(500);
            
            entity.HasIndex(e => e.DocumentId);
            entity.HasIndex(e => e.AnalysisTimestamp);
        });
    }
}



