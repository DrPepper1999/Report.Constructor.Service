using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Report.Constructor.DAL.MoscowVideoDb.Configuration;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.MoscowVideoDb;

public sealed class MoscowVideoDbContext : DbContext
{
    public MoscowVideoDbContext(DbContextOptions<MoscowVideoDbContext> options) : base(options) { }

    public DbSet<Token> Tokens { get; set; } = null!;
    public DbSet<Camera> Cameras { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserGroup> UserGroups { get; set; } = null!;
    public DbSet<ExternalService> ExternalServices { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<CameraPersonalPosition> CameraPersonalPositions { get; set; } = null!;
    public DbSet<TagObject> TagObjects { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Получение всех типов сущностей с аттрибутом MoscowVideoEntityAttribute
        var moscowVideoEntityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => t.ClrType.GetCustomAttributes<MoscowVideoEntityAttribute>().Any())
            .Select(t => t.ClrType)
            .ToHashSet();
        
        // Применение конфигураций
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly(),
            t => t.GetInterfaces()
                .Any(i => i.IsGenericType
                          && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)
                          && moscowVideoEntityTypes.Contains(i.GenericTypeArguments[0])));

        // Отключение миграций
        foreach (var moscowVideoEntityType in moscowVideoEntityTypes)
        {
            modelBuilder.Entity(moscowVideoEntityType).Metadata
                .SetIsTableExcludedFromMigrations(true);
        }
    }
}