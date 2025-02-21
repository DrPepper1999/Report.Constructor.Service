using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.MoscowVideoDb.Configuration;

public class CameraContractConfiguration : IEntityTypeConfiguration<CameraContract>
{
    public void Configure(EntityTypeBuilder<CameraContract> builder)
    {
        builder.ToTable("CameraContracts");
    }
}