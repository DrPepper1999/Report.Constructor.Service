using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.MoscowVideoDb.Configuration;

[MoscowVideoEntity]
public class CameraStateConfiguration : IEntityTypeConfiguration<CameraState>
{
    public void Configure(EntityTypeBuilder<CameraState> builder)
    {
        builder.ToTable("CameraStates");

        builder.HasKey(cs => cs.Id);
    }
}