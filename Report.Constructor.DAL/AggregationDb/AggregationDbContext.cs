using Microsoft.EntityFrameworkCore;

namespace Report.Constructor.DAL.AggregationDb;

public class AggregationDbContext : DbContext
{
    public AggregationDbContext(DbContextOptions<AggregationDbContext> options) : base(options) { }
}