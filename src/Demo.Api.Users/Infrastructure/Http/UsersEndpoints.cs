using System.Diagnostics;
using Api.Users.DTOs;
using Api.Users.UseCases;
using Common.Exceptions;
using Common.Observability;
using MediatR;

namespace Api.Users.Infrastructure.Http;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/users")
            .WithOpenApi();


        group.MapGet("", async (IMediator mediator, ILoggerFactory loggerFactory) =>
        {
            Diagnostic.AddHttpRequest();
            try
            {
                using var activity = Diagnostic.Source.StartHttpActivity("Get: /users");

                var response = await mediator.Send(GetUsersQuery.Instance);

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
                .StartHttpActivity("Get: /users/{id}")?
                .SetTag("UserId", id.ToString());

            try
            {
                var response = await mediator.Send(new GetUserQuery(id));
                return Results.Ok(response);
            }
            catch(UserNotFoundException exception)
            {
                Activity.Current.RegisterValidation<UserNotFoundException>(
                    new() { { nameof(id), id } });

                return Results.NotFound(exception.Message);
            }
            catch(Exception exception)
            {
                loggerFactory
                    .CreateLogger("NotificationsEndpoints")
                    .LogError(exception, "[WEB API][GET][BY USER ID]");

                Activity.Current.RegisterException(
                    exception,
                    new() { { nameof(id), id } });
                throw;
            }
        }).WithName("GetUser");


        group.MapGet("{id:guid}/total-notifications", async (IMediator mediator, ILoggerFactory loggerFactory, Guid id) =>
        {
            Diagnostic.AddHttpRequest();

            using var activity = Diagnostic.Source
                .StartHttpActivity("Get: /users/{id}/total-notifications")?
                .SetTag("UserId", id.ToString());

            try
            {
                var response = await mediator.Send(new GetUserNotificationsTotals(id));
                return Results.Ok(response);
            }
            catch(UserNotFoundException exception)
            {
                Activity.Current.RegisterValidation<UserNotFoundException>(
                    new() { { nameof(id), id } });

                return Results.NotFound(exception.Message);
            }
            catch(Exception exception)
            {
                loggerFactory
                    .CreateLogger("NotificationsEndpoints")
                    .LogError(exception, "[WEB API][GET TOTAL NOTIFICATIONS][BY USER ID]");

                Activity.Current.RegisterException(
                    exception,
                    new() { { nameof(id), id } });
                throw;
            }
        });


        group.MapPost("", async (IMediator mediator, ILoggerFactory loggerFactory, UserRequest request) =>
        {
            Diagnostic.AddHttpRequest();

            try
            {
                using var activity = Diagnostic.Source.StartHttpActivity("Post: /users");

                var id = await mediator.Send(new CreateUserCommand(request));

                activity?.SetTag("UserId", id.ToString());

                return Results.CreatedAtRoute(
                    "GetUser",
                    new { id },
                    id);
            }
            catch(Exception exception)
            {
                loggerFactory
                    .CreateLogger("NotificationsEndpoints")
                    .LogError(exception, "[WEB API][POST]");

                Activity.Current.RegisterException(
                    exception,
                    new() { { nameof(request.Name), request.Name } });
                throw;
            }
        });


        group.MapPut("{id:guid}", async (IMediator mediator, ILoggerFactory loggerFactory, Guid id, UserRequest request) =>
        {
            Diagnostic.AddHttpRequest();

            using var activity = Diagnostic.Source
                .StartHttpActivity("Put: /users")?
                .SetTag("UserId", id.ToString());

            try
            {
                await mediator.Send(new UpdateUserCommand(id, request));
            }
            catch(UserNotFoundException exception)
            {
                Activity.Current.RegisterValidation<UserNotFoundException>(
                    new() { { nameof(id), id } });

                return Results.NotFound(exception.Message);
            }
            catch(Exception exception)
            {
                loggerFactory
                    .CreateLogger("NotificationsEndpoints")
                    .LogError(exception, "[WEB API][PUT]");

                Activity.Current.RegisterException(
                    exception,
                    new() { { nameof(id), id } });
                throw;
            }

            return Results.NoContent();
        });


        group.MapDelete("{id:guid}", async (IMediator mediator, ILoggerFactory loggerFactory, Guid id) =>
        {
            Diagnostic.AddHttpRequest();

            using var activity = Diagnostic.Source
                .StartHttpActivity("Delete: /users")?
                .SetTag("UserId", id.ToString());

            try
            {
                await mediator.Send(new DeleteUserCommand(id));
            }
            catch(UserNotFoundException exception)
            {
                Activity.Current.RegisterValidation<UserNotFoundException>(
                    new() { { nameof(id), id } });

                return Results.NotFound(exception.Message);
            }
            catch(Exception exception)
            {
                loggerFactory
                    .CreateLogger("NotificationsEndpoints")
                    .LogError(exception, "[WEB API][DELETE]");

                Activity.Current.RegisterException(
                    exception,
                    new() { { nameof(id), id } });
                throw;
            }

            return Results.NoContent();
        });
    }
}
