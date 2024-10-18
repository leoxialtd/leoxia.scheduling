using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Leoxia.Scheduling.Domain;

internal class JobSchedulerConfiguration : IJobSchedulerConfiguration
{
    private readonly IServiceProvider _provider;

    public JobSchedulerConfiguration(IServiceProvider provider)
    {
        _provider = provider;
        ExceptionHandler = OnException;
    }

    private void OnException(Exception exception)
    {
        // Do nothing by default
    }

    
    public Action<Exception> ExceptionHandler { get; private set; }
    
    public IJobSchedulerConfiguration OnException(Action<Exception> onException)
    {
        ExceptionHandler = onException;
        return this;
    }

    public IStoppable Starts()
    {
        var timer = _provider.GetRequiredService<JobTimer>();
        timer.Start();
        return new StoppableTimer(timer);
    }
}

internal class StoppableTimer : IStoppable
{
    private readonly JobTimer _timer;

    public StoppableTimer(JobTimer timer)
    {
        _timer = timer;
    }

    public async Task Stop()
    {
        await _timer.Stop();
    }
}