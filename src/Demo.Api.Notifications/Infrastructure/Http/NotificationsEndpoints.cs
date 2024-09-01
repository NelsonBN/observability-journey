using System;
using System.Threading;
using Api.Notifications.DTOs;
using Api.Notifications.UseCases;
using BuildingBlocks.Observability;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Api.Notifications.Infrastructure.Http;

public static class NotificationsEndpoints
{
    public static void MapNotificationsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/notifications")
            .WithOpenApi();


        group.MapGet("", async (GetNotificationsQuery query, CancellationToken cancellationToken) =>
        {
            Telemetry.AddHttpRequest();

            using var activity = Telemetry.Source.StartHttpActivity("Get: /notifications");

            var response = await query.HandleAsync(cancellationToken);

            return Results.Ok(response);
        });


        group.MapGet("{id:guid}", async (GetNotificationQuery query, Guid id, CancellationToken cancellationToken) =>
        {
            Telemetry.AddHttpRequest();

            using var activity = Telemetry.Source
                .StartHttpActivity("Get: /notifications/{id}")?
                .SetTag("NotificationId", id.ToString());

            var response = await query.HandleAsync(id, cancellationToken);
            return Results.Ok(response);
        }).WithName("GetNotification");



        group.MapPost("", async (SendNotificationCommand command, NotificationRequest request, CancellationToken cancellationToken) =>
        {
            Telemetry.AddHttpRequest();

            using var activity = Telemetry.Source
                .StartHttpActivity("Post: /notifications")?
                .SetTag("UserId", request.UserId.ToString());

            var id = await command.HandleAsync(request, cancellationToken);

            activity?.SetTag("NotificationId", id.ToString());

            return Results.AcceptedAtRoute(
                "GetNotification",
                new { id },
                id);
        });
    }
}
