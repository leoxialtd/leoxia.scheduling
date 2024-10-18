using System.Collections.Concurrent;

namespace Leoxia.Scheduling.Domain;

internal class JobRun
{
    private readonly DateTimeOffset? _nextRun;

    public JobRun(Job job, DateTimeOffset now)
    {
        Job = job;
        _nextRun = job.RunScheduler.GetNextRun(now);
    }

    public bool ShouldRun(DateTimeOffset now)
    {
        return _nextRun != null && now > _nextRun;
    }

    public Job Job { get; }

    public bool IsRunning { get; set; }

    public ConcurrentBag<Task> Runs { get; } = new ConcurrentBag<Task>();
}