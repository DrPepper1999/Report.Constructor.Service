using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.MoscowVideoDb.Configuration;

public class CamerasMediaServerInfoConfiguration : IEntityTypeConfiguration<CamerasMediaServerInfo>
{
    public void Configure(EntityTypeBuilder<CamerasMediaServerInfo> builder)
    {
        builder.ToTable(nameof(CamerasMediaServerInfo));

        builder.HasKey(e => e.CameraId);
    }
}