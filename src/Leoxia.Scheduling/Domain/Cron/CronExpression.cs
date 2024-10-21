using System;

namespace Leoxia.Scheduling.Domain.Cron;

public class CronExpression
{
    public CronField Seconds { get; private set; }
    public CronField Minutes { get; private set; }
    public CronField Hours { get; private set; }
    public CronField DayOfMonth { get; private set; }
    public CronField Month { get; private set; }
    public CronField DayOfWeek { get; private set; }

    public CronExpression(string cronExpression)
    {
        var parts = cronExpression.Split(' ');
        if (parts.Length != 6)
        {
            throw new ArgumentException("Invalid cron expression format. It should consist of 6 fields.");
        }

        Seconds = ParseField(parts[0], 0, 59, "Seconds");
        Minutes = ParseField(parts[1], 0, 59, "Minutes");
        Hours = ParseField(parts[2], 0, 23, "Hours");
        DayOfMonth = ParseField(parts[3], 1, 31, "DayOfMonth");
        Month = ParseField(parts[4], 1, 12, "Month");
        DayOfWeek = ParseField(parts[5], 0, 7, "DayOfWeek");
    }

    private CronField ParseField(string field, int min, int max, string fieldName, bool canCompose = true)
    {
        if (field == "*")
        {
            return new StarField();
        }

        if (canCompose && field.Contains(","))
        {
            var parts = field.Split(',');
            return new CompositeField(parts.Select(x => ParseField(x, min, max, fieldName, false)));
        }

        if (field.Contains("-"))
        {
            var rangeParts = field.Split('-');
            var start = int.Parse(rangeParts[0]);
            var end = int.Parse(rangeParts[1]);
            if (start < min || end > max)
            {
                throw new ArgumentOutOfRangeException(nameof(field), $"cron expression is invalid, field {fieldName} range should be between {min} {max}");
            }

            return new RangeField(start, end);
        }
        if (field.Contains("/"))
        {
            var intervalParts = field.Split('/');
            return new IntervalField(int.Parse(intervalParts[0] == "*" ? "0" : intervalParts[0]), int.Parse(intervalParts[1]));
        }

        return new SpecificValueField(int.Parse(field));
    }

    public DateTimeOffset GetNextTrigger(DateTimeOffset now)
    {
        DateTimeOffset next = now.Add(TimeSpan.FromSeconds(1));

        while (true)
        {
            if (!Seconds.Matches(next.Second))
            {
                next = next.AddSeconds(Seconds.Next(next.Second, 0, 59) - next.Second);
                continue;
            }

            if (!Minutes.Matches(next.Minute))
            {
                next = next.AddMinutes(Minutes.Next(next.Minute, 0, 59) - next.Minute);
                continue;
            }

            if (!Hours.Matches(next.Hour))
            {
                next = next.AddHours(Hours.Next(next.Hour, 0, 23) - next.Hour);
                continue;
            }

            if (!DayOfMonth.Matches(next.Day))
            {
                next = next.AddDays(DayOfMonth.Next(next.Day, 1, 31) - next.Day);
                continue;
            }

            if (!Month.Matches(next.Month))
            {
                next = next.AddMonths(Month.Next(next.Month, 1, 12) - next.Month);
                continue;
            }

            var dayOfWeek = next.DayOfWeek == 0 ? 7 : (int)next.DayOfWeek;
            if (!DayOfWeek.Matches(dayOfWeek))
            {
                var daysToAdd = DayOfWeek.Next(dayOfWeek, 1, 7) - dayOfWeek;
                next = next.AddDays(daysToAdd);
                continue;
            }

            return next;
        }
    }
}