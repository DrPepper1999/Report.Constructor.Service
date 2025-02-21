using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.MoscowVideoDb.Configuration;

public class CameraConfiguration : IEntityTypeConfiguration<Camera>
{
    public void Configure(EntityTypeBuilder<Camera> builder)
    {
        builder.ToTable("Cameras");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.PlaceInExploitationDate, "PlaceInExplotationDate");
        builder.HasIndex(e => e.PlaceInExploitationDate, "missing_index_312_311_Cameras");
        builder.HasIndex(e => e.CameraModelId, "ix_Cameras_CameraModelId_includes");
            
        builder.Property(e => e.PlaceInExploitationDate).HasColumnType("datetime");
            
        builder.HasMany(d => d.CameraSurveillanceObjects).WithMany(p => p.Cameras)
            .UsingEntity<Dictionary<string, object>>(
                "CamerasCameraSurveillanceObject",
                r => r.HasOne<CameraSurveillanceObject>().WithMany()
                    .HasForeignKey("SurveillanceObjectId")
                    .HasConstraintName("FK_Cameras_CameraSurveillanceObjects_CameraSurveillanceObjects"),
                l => l.HasOne<Camera>().WithMany()
                    .HasForeignKey("CameraId")
                    .HasConstraintName("FK_Cameras_CameraSurveillanceObjects_Cameras"),
                j =>
                {
                    j.HasKey("CameraId", "SurveillanceObjectId");
                    j.ToTable("Cameras_CameraSurveillanceObjects");
                    j.HasIndex(new[] { "CameraId", "SurveillanceObjectId" }, "i_Cameras_CameraSurveillanceObjects_CameraId_SurveillanceObjectId");
                });
    }
}