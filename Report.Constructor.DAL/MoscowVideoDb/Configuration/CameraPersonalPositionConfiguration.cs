using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.MoscowVideoDb.Configuration;

public class CameraPersonalPositionConfiguration : IEntityTypeConfiguration<CameraPersonalPosition>
{
    public void Configure(EntityTypeBuilder<CameraPersonalPosition> builder)
    {
        builder.ToTable("CameraPersonalPositions");
            
        builder.ToTable("CameraPositions_PersonalPositions").UseTptMappingStrategy();
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.HasOne(e => e.IdNavigation).WithOne(b => b.CameraPositionsPersonalPosition)
            .HasForeignKey<CameraPersonalPosition>(e => e.Id)
            .HasConstraintName("FK_PersonalPositions_inherits_CameraPosition");
    }
}