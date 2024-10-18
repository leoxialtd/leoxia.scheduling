using System.Collections.Concurrent;
using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain;

internal class JobRepository : IJobRepository
{
    private readonly ConcurrentBag<Job> _jobs = new ConcurrentBag<Job>();

    public void Add(Job job)
    {
        _jobs.Add(job);
    }

    public IEnumerable<IJob> GetJobs()
    {
        return _jobs.ToArray().Cast<IJob>();
    }
}