using Microsoft.EntityFrameworkCore;
using Report.Constructor.DAL.ReportsDb.Configurations;
using Report.Constructor.DAL.ReportsDb.Entities;

namespace Report.Constructor.DAL.ReportsDb;

public class ReportsDbContext : DbContext
{
    public ReportsDbContext(DbContextOptions<ReportsDbContext> options) : base(options) { }


    public virtual DbSet<UserAction> UserActions { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserActionConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
