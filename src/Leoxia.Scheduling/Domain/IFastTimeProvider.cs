namespace Leoxia.Scheduling.Domain;

internal interface IFastTimeProvider
{
    DateTimeOffset UtcNow();
}