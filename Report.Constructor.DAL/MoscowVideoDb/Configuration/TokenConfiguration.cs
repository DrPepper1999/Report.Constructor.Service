using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.MoscowVideoDb.Configuration;

public class TokenConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.ToTable("Tokens");

        builder.HasKey(e => new { e.Id, e.CreationDate });

        builder.Property(e => e.ActivityType)
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.HasOne(d => d.Camera)
            .WithMany(p => p.Tokens)
            .HasForeignKey(d => d.CameraId)
            .HasConstraintName("FK_Tokens_Cameras");

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .HasConstraintName("FK_Tokens_Users1");
    }
}