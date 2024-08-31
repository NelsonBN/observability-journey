using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Api.Notifications.Domain;
using Api.Notifications.DTOs;
using BuildingBlocks.Observability;
using Quartz;

namespace Api.Notifications.UseCases;

public sealed class ReportGenerateJob(
    ILogger<ReportGenerateJob> logger,
    IReportsRepository repository,
    IStorageService storage) : IJob
{
    private readonly ILogger<ReportGenerateJob> _logger = logger;
    private readonly IReportsRepository _repository = repository;
    private readonly IStorageService _storage = storage;

    private static readonly JsonSerializerOptions _jsonOpetions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("[JOB][REPORT GENERATE] Starting...");

        try
        {
            var reportState = await _repository.GetStateAsync(context.CancellationToken);
            if(reportState is null)
            {
                reportState = new()
                {
                    LastGeneratedAt = DateTime.UtcNow.Date.AddDays(-1)
                };
                await _repository.AddAsync(reportState, context.CancellationToken);
            }

            var report = new NotificationsReport
            {
                StartDateTime = reportState.LastGeneratedAt,
                EndDateTime = DateTime.UtcNow
            };

            var notifications = await _repository.NotificationSummariesAsync(
                report.StartDateTime,
                report.EndDateTime,
                context.CancellationToken);


            foreach(var notification in notifications)
            {
                if(notification.EmailNotificationStatus == NotificationStatus.Sent)
                {
                    report.TotalEmailsSent++;
                }
                else if(notification.EmailNotificationStatus == NotificationStatus.Failed)
                {
                    report.TotalEmailsFailed++;
                }

                if(notification.PhoneNotificationStatus == NotificationStatus.Sent)
                {
                    report.TotalSMSsSent++;
                }
                else if(notification.PhoneNotificationStatus == NotificationStatus.Failed)
                {
                    report.TotalSMSsFailed++;
                }
            }

            using var reportContent = new MemoryStream(
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(report, _jsonOpetions)));

            await _storage.SaveAsync(
                reportContent,
                $"{report.EndDateTime:yyyyMMddHHmmss}.json",
                context.CancellationToken);

            reportState.LastGeneratedAt = report.EndDateTime;

            await _repository.UpdateAsync(reportState, context.CancellationToken);

            _logger.LogInformation("[JOB][REPORT GENERATE] Created report");
        }
        catch(Exception exception)
        {
            Activity.Current.RegisterException(exception);
            throw;
        }
    }
}
