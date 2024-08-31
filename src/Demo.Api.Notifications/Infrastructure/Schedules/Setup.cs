using Api.Notifications.UseCases;
using Quartz;
using Quartz.Listener;

namespace Api.Notifications.Infrastructure.Schedules;

public static class Setup
{
    public static IServiceCollection AddSchedules(this IServiceCollection services)
        => services
            .AddQuartzHostedService(o => o.WaitForJobsToComplete = true)
            .AddQuartz(config =>
            {
                var cronSchedule = "*/10 * * * * ? *";

                var jobKey = new JobKey(typeof(ReportGenerateJob).Name);

                config.AddJob<ReportGenerateJob>(jobKey, job => job
                    .UsingJobData("ScheduleCron", cronSchedule)
                    .DisallowConcurrentExecution());

                // Every 10 seconds
                config.AddTrigger(trigger => trigger
                      .ForJob(jobKey)
                      .WithIdentity(jobKey.Name)
                      .StartNow()
                      .WithCronSchedule(cronSchedule));

                config.AddJobListener<EnrichJobListener>();
            });

    private sealed class EnrichJobListener : JobListenerSupport
    {
        public override string Name => typeof(EnrichJobListener).Name;

        public override Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            var next = context.Trigger.GetNextFireTimeUtc();
            if(next is not null)
            {
                context.JobDetail.JobDataMap.Add("ScheduleNext", next.Value.ToString("yyyy-MM-dd HH:mm:ss zzz"));
            }
            return Task.CompletedTask;
        }
    }
}
