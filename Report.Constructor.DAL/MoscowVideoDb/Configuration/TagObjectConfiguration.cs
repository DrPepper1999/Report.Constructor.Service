using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.MoscowVideoDb.Configuration;

public class TagObjectConfiguration : IEntityTypeConfiguration<TagObject>
{
    public void Configure(EntityTypeBuilder<TagObject> builder)
    {
        builder.ToTable("Tag_Objects");
    }
}