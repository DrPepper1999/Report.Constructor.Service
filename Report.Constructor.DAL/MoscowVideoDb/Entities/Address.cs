using Report.Constructor.DAL.MoscowVideoDb.Configuration;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public sealed class Address
{
    public int Id { get; set; }
    public int DistrictId { get; set; }
    public int? MunicipalDistrictId { get; set; }
}