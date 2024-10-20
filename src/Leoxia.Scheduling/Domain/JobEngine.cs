using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.Logging;

namespace Leoxia.Scheduling.Domain;

internal class JobEngine
{
    private readonly ILogger<JobEngine> _logger;
    private readonly IJobRunRepository _repository;
    private readonly JobSchedulerConfiguration _configuration;
    private readonly IFastTimeProvider _timeProvider;

    public JobEngine(
        ILogger<JobEngine> logger,
        IJobRunRepository repository,
        JobSchedulerConfiguration configuration,
        IFastTimeProvider timeProvider)
    {
        _logger = logger;
        _repository = repository;
        _configuration = configuration;
        _timeProvider = timeProvider;
    }

    public bool IsRunning { get; }

    public void Run(DateTimeOffset now)
    {
        var runs = _repository.GetJobRuns();
        foreach (var run in runs)
        {
            if (run.ShouldRun(now))
            {
                run.SetNextRun(_timeProvider.UtcNow());
                _logger.LogDebug($"Run {run} running...");
                Task.Run(async () =>
                {
                    try
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
                    }
                    catch (Exception e)
                    {
                        _configuration.ExceptionHandler(e, $"While running {run}");
                    }
                });
            }
        }
    }
}