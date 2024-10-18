using Leoxia.Scheduling.Abstractions;
using Leoxia.Scheduling.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Leoxia.Scheduling;

public static class ServiceExtensions
{
    public static IServiceCollection AddJobScheduler(this IServiceCollection services)
    {
        services.AddSingleton<JobSchedulerConfiguration>();
        services.AddSingleton<IJobSchedulerConfiguration>(sp => sp.GetRequiredService<JobSchedulerConfiguration>());
        services.AddSingleton<FastTimeProvider>();
        services.AddSingleton<IFastTimeProvider>(sp => sp.GetRequiredService<FastTimeProvider>());
        services.AddSingleton<IJobRunRepository, JobRunRepository>();
        services.AddSingleton<JobEngine>();
        services.AddSingleton<JobTimer>();
        services.AddSingleton<JobRepository>();
        services.AddSingleton<IJobRepository>(sp => sp.GetRequiredService<JobRepository>());
        services.AddSingleton<IJobScheduler, JobScheduler>();
        services.AddHostedService<SchedulingService>();
        return services;
    }
}