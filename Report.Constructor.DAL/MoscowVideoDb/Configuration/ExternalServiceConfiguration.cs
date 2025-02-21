using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.MoscowVideoDb.Configuration;

public class ExternalServiceConfiguration : IEntityTypeConfiguration<ExternalService>
{
    public void Configure(EntityTypeBuilder<ExternalService> builder)
    {
        builder.ToTable("ExternalServices");
            
        builder.HasKey(e => new { e.Id });

        builder.Property(e => e.Id).HasColumnName("Id");
        builder.Property(e => e.Key).HasColumnName("Key");
        builder.Property(e => e.Name).HasColumnName("Name");
    }
}