using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Constructor.DAL.ReportOrdersDb.Entities;

namespace Report.Constructor.DAL.ReportOrdersDb.Configurations;

internal sealed class ReportOrderConfiguration : IEntityTypeConfiguration<ReportOrderEntity>
{
    public void Configure(EntityTypeBuilder<ReportOrderEntity> builder)
    {
        builder.ToTable("ReportOrder");
        
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.UserId);
    }
}