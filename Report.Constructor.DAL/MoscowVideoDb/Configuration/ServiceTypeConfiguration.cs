using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.MoscowVideoDb.Configuration;

[MoscowVideoEntity]
public class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
{
    public void Configure(EntityTypeBuilder<ServiceType> builder)
    {
        builder.ToTable("ServiceTypes");

        builder.HasKey(st => st.Id);
    }
}