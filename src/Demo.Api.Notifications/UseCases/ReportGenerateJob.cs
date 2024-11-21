using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Api.Notifications.Domain;
using Api.Notifications.DTOs;
using BuildingBlocks.Contracts;
using BuildingBlocks.Contracts.Abstractions;
using BuildingBlocks.Observability;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Api.Notifications.UseCases;

public sealed class ReportGenerateJob(
    ILogger<ReportGenerateJob> logger,
    IReportsRepository repository,
    IStorageService storage,
    IPublisher publisher) : IJob
{
    private readonly ILogger<ReportGenerateJob> _logger = logger;
    private readonly IReportsRepository _repository = repository;
    private readonly IStorageService _storage = storage;
    private readonly IPublisher _publisher = publisher;

    private static readonly JsonSerializerOptions _jsonOpetions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("[JOB][REPORT] Starting...");

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

            var fileName = $"{report.EndDateTime:yyyyMMddHHmmss}.json";

            await _storage.SaveAsync(
                reportContent,
                fileName,
                context.CancellationToken);

            reportState.LastGeneratedAt = report.EndDateTime;

            await _publisher.Publish(new Message()
            {
                Context = "reports",
                Type = "email.report",
                AggregateId = Guid.NewGuid(),
                Data = new Dictionary<string, object>
                {
                    ["Email"] = "admin@fake.fk",
                    ["Body"] = "Notifications report",
                    ["Attachment"] = fileName,
                }
            });

            await _repository.UpdateAsync(reportState, context.CancellationToken);

            _logger.LogInformation("[JOB][REPORT] Created report {FileName}", fileName);
        }
        catch(Exception exception)
        {
            Activity.Current.RegisterException(exception);
            throw;
        }
    }
}
