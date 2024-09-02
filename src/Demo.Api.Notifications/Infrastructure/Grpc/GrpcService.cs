using System;
using System.Threading.Tasks;
using Api.Notifications.UseCases;
using BuildingBlocks.Contracts.Notifications;
using BuildingBlocks.Observability;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Api.Notifications.Infrastructure.Grpc;

public sealed class GrpcService(
    ILogger<GrpcService> logger,
    GetNotificationsTotalsQuery query) : Greeter.GreeterBase
{
    private readonly ILogger<GrpcService> _logger = logger;
    private readonly GetNotificationsTotalsQuery _query = query;

    public override async Task<NotificationsTotalsResponse> GetNotificationsTotals(NotificationsTotalsRequest request, ServerCallContext context)
    {
        Telemetry.IncreaseGrpcRequest();
        _logger.LogInformation("[INFRASTRUCTURE][GRPC][GET NOTIFICATIONS TOTALS] UserId '{UserId}'", request.UserId); // TODO: to move to tracing

        var userId = Guid.Parse(request.UserId);
        var total = await _query.HandleAsync(userId, context.CancellationToken);

        return total;
    }
}
