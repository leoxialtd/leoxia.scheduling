namespace Leoxia.Scheduling.Domain.Cron;

public class StarField : CronField
{
    public override bool Matches(int value) => true;
    public override int Next(int currentValue, int minValue, int maxValue) => currentValue + 1;
}