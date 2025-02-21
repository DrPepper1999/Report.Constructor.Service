namespace Report.Constructor.DAL.ReportsDb.Entities;

public class UserAction
{
    public Guid UserId { get; set; }

    public DateTime Date { get; set; }

    public int ActionCount { get; set; }
}
