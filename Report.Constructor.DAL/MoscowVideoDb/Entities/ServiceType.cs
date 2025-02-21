using Report.Constructor.DAL.MoscowVideoDb.Configuration;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public class ServiceType
{
    public int Id { get; set; }

    public required string Name { get; set; }
}