namespace Leoxia.Scheduling.Domain.Cron;

public abstract class CronField
{
    public abstract bool Matches(int value);
    public abstract int Next(int currentValue, int minValue, int maxValue);
}