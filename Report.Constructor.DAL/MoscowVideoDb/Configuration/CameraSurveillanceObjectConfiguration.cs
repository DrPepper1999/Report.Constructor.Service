using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.MoscowVideoDb.Configuration;

public class CameraSurveillanceObjectConfiguration : IEntityTypeConfiguration<CameraSurveillanceObject>
{
    public void Configure(EntityTypeBuilder<CameraSurveillanceObject> builder)
    {
        builder.ToTable("CameraSurveillanceObjects");
    }
}