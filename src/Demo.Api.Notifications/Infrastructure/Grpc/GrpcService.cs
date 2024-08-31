using Api.Notifications.UseCases;
using BuildingBlocks.Observability;
using Grpc.Core;
using MediatR;
using Notifications;

namespace Api.Notifications.Infrastructure.Grpc;

public sealed class GrpcService(
    IMediator mediator,
    ILogger<GrpcService> logger) : Greeter.GreeterBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<GrpcService> _logger = logger;


    public override async Task<NotificationsTotalsResponse> GetNotificationsTotals(NotificationsTotalsRequest request, ServerCallContext context)
    {
        Telemetry.AddGrpcRequest();
        _logger.LogInformation("[INFRASTRUCTURE][GRPC][GET NOTIFICATIONS TOTALS] UserId '{UserId}'", request.UserId); // TODO: to move to tracing

        var userId = Guid.Parse(request.UserId);
        var total = await _mediator.Send(new GetNotificationsTotalsQuery(userId), context.CancellationToken);

        return total;
    }
}
