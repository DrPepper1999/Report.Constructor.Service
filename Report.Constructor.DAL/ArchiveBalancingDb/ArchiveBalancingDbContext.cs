using Microsoft.EntityFrameworkCore;
using Report.Constructor.DAL.ArchiveBalancingDb.Entities;

namespace Report.Constructor.DAL.ArchiveBalancingDb;

public class ArchiveBalancingDbContext : DbContext
{
    public ArchiveBalancingDbContext(DbContextOptions<ArchiveBalancingDbContext> options) : base(options) { }

    public DbSet<ArchiveTask> ArchiveTasks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArchiveTask>(builder =>
        {
            builder.ToTable("ArchiveTask", t => t.ExcludeFromMigrations());
            
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.FileSize);
        });
    }
}