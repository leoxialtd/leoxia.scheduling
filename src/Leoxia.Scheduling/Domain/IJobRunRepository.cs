using Leoxia.Scheduling.Abstractions;

namespace Leoxia.Scheduling.Domain;

internal interface IJobRunRepository
{
    IEnumerable<JobRun> GetJobRuns();
}

internal class JobRunRepository : IJobRunRepository
{
    private readonly List<JobRun> _runs;

    public JobRunRepository(IJobRepository repository, IFastTimeProvider provider)
    {
        var now = provider.UtcNow();
        _runs = repository.GetJobs()
            .Select(job => new JobRun((Job)job, now)).ToList();
    }

    public IEnumerable<JobRun> GetJobRuns()
    {
        return _runs;
    }
}