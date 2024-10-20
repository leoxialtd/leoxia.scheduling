namespace Leoxia.Scheduling.Domain.Cron;

public class RangeField : CronField
{
    private readonly int _start;
    private readonly int _end;

    public RangeField(int start, int end)
    {
        _start = start;
        _end = end;
    }

    public override bool Matches(int value) => value >= _start && value <= _end;
    public override int Next(int currentValue, int minValue, int maxValue)
    {
        if (currentValue < _start)
            return _start;
        if (currentValue >= _end)
            return minValue;
        return currentValue + 1;
    }
}