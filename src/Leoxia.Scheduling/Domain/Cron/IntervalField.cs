namespace Leoxia.Scheduling.Domain.Cron;

public class IntervalField : CronField
{
    private readonly int _interval;
    private readonly int _start;

    public IntervalField(int start, int interval)
    {
        _start = start;
        _interval = interval;
    }

    public override bool Matches(int value) => (value - _start) % _interval == 0;
    public override int Next(int currentValue, int minValue, int maxValue)
    {
        int nextValue;
        if (currentValue < _interval)
        {
            nextValue = _interval;
        }
        else
        {
            nextValue = currentValue - (currentValue % _interval) + _interval;
        }

        return nextValue <= maxValue ? nextValue : minValue;
    }
}