using System.Diagnostics;
using Api.Notifications.Domain;
using Api.Notifications.DTOs;
using Api.Notifications.UseCases;
using Common.Exceptions;
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

            try
            {
                using var activity = Diagnostic.Source.StartHttpActivity("Get: /notifications");

                var response = await mediator.Send(GetNotificationsQuery.Instance);

                return Results.Ok(response);
            }
            catch(Exception exception)
            {
                loggerFactory
                    .CreateLogger("NotificationsEndpoints")
                    .LogError(exception, "[WEB API][GET]");

                Activity.Current.RegisterException(exception);
                throw;
            }
        });


        group.MapGet("{id:guid}", async (IMediator mediator, ILoggerFactory loggerFactory, Guid id) =>
        {
            Diagnostic.AddHttpRequest();

            using var activity = Diagnostic.Source
                .StartHttpActivity("Get: /notifications/{id}")?
                .SetTag("NotificationId", id.ToString());

            try
            {
                var response = await mediator.Send(new GetNotificationQuery(id));
                return Results.Ok(response);
            }
            catch(NotificationNotFoundException exception)
            {
                Activity.Current.RegisterValidation<NotificationNotFoundException>(
                    new() { { nameof(id), id } });

                return Results.NotFound(exception.Message);
            }
            catch(Exception exception)
            {
                loggerFactory
                    .CreateLogger("NotificationsEndpoints")
                    .LogError(exception, "[WEB API][GET][BY ID]");

                Activity.Current.RegisterException(
                    exception,
                    new() { { nameof(id), id } });
                throw;
            }
        }).WithName("GetNotification");



        group.MapPost("", async (IMediator mediator, ILoggerFactory loggerFactory, NotificationRequest request) =>
        {
            Diagnostic.AddHttpRequest();

            using var activity = Diagnostic.Source
                .StartHttpActivity("Post: /notifications")?
                .SetTag("UserId", request.UserId.ToString());

            try
            {
                var id = await mediator.Send(new SendNotificationCommand(request));

                activity?.SetTag("NotificationId", id.ToString());

                return Results.AcceptedAtRoute(
                    "GetNotification",
                    new { id },
                    id);
            }
            catch(UserNotFoundException exception)
            {
                Activity.Current.RegisterValidation<UserNotFoundException>(
                    new() { { nameof(request.UserId), request.UserId } });

                return Results.NotFound(exception.Message);
            }
            catch(Exception exception)
            {
                loggerFactory
                    .CreateLogger("NotificationsEndpoints")
                    .LogError(exception, "[WEB API][POST]");

                Activity.Current.RegisterException(
                    exception,
                    new() { { nameof(request.UserId), request.UserId } });
                throw;
            }
        });
    }
}
