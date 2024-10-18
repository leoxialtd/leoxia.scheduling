using Microsoft.Extensions.Logging;

namespace Leoxia.Scheduling.Abstractions;

public interface IJobSchedulerConfiguration
{
    IJobSchedulerConfiguration WithLoggerFactory(ILogger<IJobScheduler> logger);

    IJobSchedulerConfiguration OnException(Action<Exception> onException);


    /// <summary>
    /// Start the inner timer for scheduling jobs immediately.
    /// Otherwise, it will be started by the hosted service.
    /// Should be set to true when not hosted in ASP.NET Core application.
    /// </summary>
    /// <returns>Instance to stop the scheduler.</returns>
    IStoppable Starts();
}