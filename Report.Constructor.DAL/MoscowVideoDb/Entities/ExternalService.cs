using System.ComponentModel.DataAnnotations.Schema;
using Report.Constructor.DAL.MoscowVideoDb.Configuration;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public sealed record ExternalService
{
    public Guid Id { get; init; }
    
    public required string Key { get; init; }
    
    public required string Name { get; init; }
    
    [NotMapped]
    public Guid GuidKey => Guid.TryParse(Key, out var value) ? value : Guid.Empty;
}