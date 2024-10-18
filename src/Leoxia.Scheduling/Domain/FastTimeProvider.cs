namespace Leoxia.Scheduling.Domain;

public class FastTimeProvider : IFastTimeProvider
{
    private DateTimeOffset _utcNow;

    public FastTimeProvider()
    {
        _utcNow = DateTime.UtcNow;
    }

    public void Set(DateTimeOffset now)
    {
        _utcNow = now;
    }

    public DateTimeOffset UtcNow()
    {
        return _utcNow;
    }
}