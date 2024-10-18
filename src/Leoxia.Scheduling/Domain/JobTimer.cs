using Microsoft.Extensions.Logging;

namespace Leoxia.Scheduling.Domain;

/// <summary>
/// Starts and stops the timer loop.
/// </summary>
internal class JobTimer
{
    private readonly IFastTimeProvider _timeProvider;
    private readonly JobEngine _engine;
    private readonly JobSchedulerConfiguration _configuration;
    private readonly Timer _timer;
    private volatile bool _running;

    public JobTimer(
        IFastTimeProvider timeProvider,
        JobEngine engine,
        JobSchedulerConfiguration configuration)
    {
        _timeProvider = timeProvider;
        _engine = engine;
        _configuration = configuration;
        _timer = new Timer(OnTimerTick, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    private void OnTimerTick(object? state)
    {
        if (_running)
        {
            try
            {
                var now = _timeProvider.UtcNow();
                _engine.Run(now);
            }
            catch (Exception e)
            {
                _configuration.ExceptionHandler(e);
            }
        }
    }

    public void Start()
    {
        _running = true;
        _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
        _configuration.Logger.LogInformation("[Scheduling] Timer started");
    }

    public async Task Stop()
    {
        _configuration.Logger.LogInformation("[Scheduling] Timer stopped");
        _running = false;
        while (_engine.IsRunning)
        {
            await Task.Delay(50);
        }

        await _timer.DisposeAsync();
    }
}