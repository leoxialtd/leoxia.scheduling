namespace Leoxia.Scheduling.Domain.Cron;

public class SpecificValueField : CronField
{
    private readonly int _value;
    public SpecificValueField(int value)
    {
        _value = value;
    }

    public override bool Matches(int value) => _value == value;
    public override int Next(int currentValue, int minValue, int maxValue) => _value > currentValue ? _value : currentValue + 1;
}