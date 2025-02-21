using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Constructor.DAL.ReportsDb.Entities;

namespace Report.Constructor.DAL.ReportsDb.Configurations;

public class UserActionConfiguration : IEntityTypeConfiguration<UserAction>
{
    public void Configure(EntityTypeBuilder<UserAction> builder)
    {
        builder.HasKey(e => new { e.Date, e.UserId }).HasName("PK_dbo.UserActions");

        builder.Property(e => e.Date).HasColumnType("datetime");
    }
}