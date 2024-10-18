
using Leoxia.Scheduling.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NUnit.Framework;

namespace Leoxia.Scheduling.Tests
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class SchedulerTests
    {
        private readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceProvider _serviceProvider;
        private readonly IStoppable _jobTimer;

        public SchedulerTests()
        {
            _logger.Info("Scheduler Test created");
            var loggerFactory = new NLogLoggerFactory();
            var services = new ServiceCollection();
            services.AddJobScheduler();
            _serviceProvider = services.BuildServiceProvider();
            _jobTimer = _serviceProvider.UseJobScheduler(_ => { })
                .LogProgress(loggerFactory.CreateLogger<IJobScheduler>())
                .Starts();
        }

        [Test]
        public void Scheduled_job_should_be_scheduled()
        {
            // GIVEN Scheduler
            var scheduler = BuildScheduler();

            var mre = new ManualResetEvent(false);

            // WHEN a job is scheduled
            scheduler.Schedule<MyJob>()
                .EverySeconds()
                .ThenRun(_ => { mre.Set(); })
                .Build();

            // THEN it should be invoked
            var invoked = mre.WaitOne(TimeSpan.FromSeconds(2));
            Assert.That(invoked, Is.True);
        }

        [Test]
        public void Scheduled_jobs_should_be_part_of_repository()
        {
            // GIVEN a Scheduler and Job repo
            var scheduler = BuildScheduler();
            var repository = _serviceProvider.GetRequiredService<IJobRepository>();

            // WHEN a job is scheduled
            scheduler.Schedule<MyJob>().Daily();

            // THEN it should be present in repo
            var job = repository.GetJobs().FirstOrDefault();
            Assert.That(job, Is.Not.Null);
            Assert.That(job!.Name, Is.EqualTo("MyJob"));
        }


        private IJobScheduler BuildScheduler()
        {
            return _serviceProvider.GetRequiredService<IJobScheduler>();
        }

        [TearDown]
        public async Task TestTearDown()
        {
            await _jobTimer.Stop();
            await _serviceProvider.DisposeAsync();
        }
    }

    public class MyJob : IInvocable
    {
        private static ManualResetEvent mre = new ManualResetEvent(false);

        public Task Invoke()
        {
            mre.Set();
            return Task.CompletedTask;
        }

        public static bool WaitForInvocation(TimeSpan span)
        {
            return mre.WaitOne(span);
        }
    }
}