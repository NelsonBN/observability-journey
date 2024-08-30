using Api.Notifications.DTOs;
using Api.Notifications.UseCases;
using Common.Observability;
using MediatR;

namespace Api.Notifications.Infrastructure.Http;

public static class NotificationsEndpoints
{
    public static void MapNotificationsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/notifications")
            .WithOpenApi();


        group.MapGet("", async (IMediator mediator, ILoggerFactory loggerFactory) =>
        {
            Diagnostic.AddHttpRequest();

            using var activity = Diagnostic.Source.StartHttpActivity("Get: /notifications");

            var response = await mediator.Send(GetNotificationsQuery.Instance);

            return Results.Ok(response);
        });


        group.MapGet("{id:guid}", async (IMediator mediator, ILoggerFactory loggerFactory, Guid id) =>
        {
            Diagnostic.AddHttpRequest();

            using var activity = Diagnostic.Source
                .StartHttpActivity("Get: /notifications/{id}")?
                .SetTag("NotificationId", id.ToString());

            var response = await mediator.Send(new GetNotificationQuery(id));
            return Results.Ok(response);
        }).WithName("GetNotification");



        group.MapPost("", async (IMediator mediator, ILoggerFactory loggerFactory, NotificationRequest request) =>
        {
            Diagnostic.AddHttpRequest();

            using var activity = Diagnostic.Source
                .StartHttpActivity("Post: /notifications")?
                .SetTag("UserId", request.UserId.ToString());

            var id = await mediator.Send(new SendNotificationCommand(request));

            activity?.SetTag("NotificationId", id.ToString());

            return Results.AcceptedAtRoute(
                "GetNotification",
                new { id },
                id);
        });
    }
}
