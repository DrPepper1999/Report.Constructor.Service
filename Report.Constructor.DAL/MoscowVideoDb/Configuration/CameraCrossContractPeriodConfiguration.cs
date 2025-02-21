using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.MoscowVideoDb.Configuration;

public class CameraCrossContractPeriodConfiguration : IEntityTypeConfiguration<CameraCrossContractPeriod>
{
    public void Configure(EntityTypeBuilder<CameraCrossContractPeriod> builder)
    {
        builder.ToTable("CameraCrossContractPeriods");
            
        builder.HasKey(e => new { e.CameraId, e.CameraContractPeriodId, e.DateStart });
            
        builder.HasOne(d => d.CameraContractPeriod).WithMany(p => p.CameraCrossContractPeriods)
            .HasForeignKey(d => d.CameraContractPeriodId)
            .HasConstraintName("FK_CameraCrossContractPeriods_CameraContractPeriods");
    }
}