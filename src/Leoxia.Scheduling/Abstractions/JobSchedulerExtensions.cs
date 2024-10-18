namespace Leoxia.Scheduling.Abstractions;

public static class JobSchedulerExtensions
{
    public static IJobBuilder Schedule<T>(this IJobScheduler scheduler) where T : IInvocable
    {
        return scheduler.Schedule(typeof(T));
    }

    // TODO: fix this to support current usage
    //public static IJobBuilder Schedule(this IJobScheduler scheduler, Func<Task> asyncTaskProvider)
    //{
    //    return scheduler.Schedule();
    //}

    //public static IJobBuilder Schedule(this IJobScheduler scheduler, Action actionToSchedule)
    //{
    //    return scheduler.Schedule();
    //}

    public static IJobBuilder EverySeconds(this IJobBuilder scheduler, int seconds = 1)
    {
        return scheduler.Every(TimeSpan.FromSeconds(seconds));
    }

    public static IJobBuilder Daily(this IJobBuilder scheduler, int days = 1)
    {
        return scheduler.Every(TimeSpan.FromDays(days));
    }

    public static IJobBuilder Once(this IJobBuilder scheduler)
    {
        return scheduler.Times(1);
    }
}