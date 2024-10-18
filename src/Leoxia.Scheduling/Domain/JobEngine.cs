using Microsoft.Extensions.Logging;

namespace Leoxia.Scheduling.Domain;

internal class JobEngine
{
    private readonly ILogger<JobEngine> _logger;
    private readonly IJobRunRepository _repository;
    private readonly JobSchedulerConfiguration _configuration;

    public JobEngine(
        ILogger<JobEngine> logger,
        IJobRunRepository repository,
        JobSchedulerConfiguration configuration)
    {
        _logger = logger;
        _repository = repository;
        _configuration = configuration;
    }

    public bool IsRunning { get; }

    public void Run(DateTimeOffset now)
    {
        var runs = _repository.GetJobRuns();
        foreach (var run in runs)
        {
            if (run.ShouldRun(now))
            {
                _logger.LogDebug($"Run {run} running...");
                Task.Run(async () =>
                {
                    using (var invocable = run.Job.Resolver.Resolve(run.Job))
                    {
                        _logger.LogDebug($"[Scheduling] Run {run}. Invoking job...");
                        await invocable.Invocable.Invoke();
                        _logger.LogDebug($"[Scheduling] Run {run}. Job invoked.");
                        foreach (var action in run.Job.ExecutionQueue)
                        {
                            action(invocable.Invocable);
                        }

                        _logger.LogDebug($"[Scheduling] Run {run}. Queued actions invoked.");
                    }

                    //run.SetLastRun(_timeProvider.UtcNow());
                });
            }
        }
    }
}