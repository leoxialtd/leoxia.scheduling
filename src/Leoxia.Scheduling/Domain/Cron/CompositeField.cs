namespace Leoxia.Scheduling.Domain.Cron;

internal class CompositeField : CronField
{
    private readonly CronField[] _fields;
    private CronField _field;

    public CompositeField(IEnumerable<CronField> fields)
    {
        _fields = fields.ToArray();
        _field = fields.First();
    }

    public override bool Matches(int value)
    {
        foreach (var field in _fields)
        {
            if (field.Matches(value))
            {
                _field = field;
                return true;
            }
        }

        return false;
    }

    public override int Next(int currentValue, int minValue, int maxValue)
    {
        return _field.Next(currentValue, minValue, maxValue);
    }
}