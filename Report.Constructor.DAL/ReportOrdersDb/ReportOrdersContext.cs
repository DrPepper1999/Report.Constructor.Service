using Microsoft.EntityFrameworkCore;
using Report.Constructor.DAL.ReportOrdersDb.Configurations;
using Report.Constructor.DAL.ReportOrdersDb.Entities;

namespace Report.Constructor.DAL.ReportOrdersDb;

public class ReportOrdersContext : DbContext
{
    public ReportOrdersContext(DbContextOptions<ReportOrdersContext> options) : base(options) { }

    public DbSet<ReportOrderEntity> ReportOrders { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ReportOrderConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}