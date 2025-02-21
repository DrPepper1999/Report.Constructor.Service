using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.MoscowVideoDb.Configuration;

public class CameraContractPeriodConfiguration : IEntityTypeConfiguration<CameraContractPeriod>
{
    public void Configure(EntityTypeBuilder<CameraContractPeriod> builder)
    {
        builder.ToTable("CameraContractPeriods");
    }
}