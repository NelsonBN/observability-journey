using Api.Users.DTOs;
using Api.Users.UseCases;
using BuildingBlocks.Observability;
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

            using var activity = Diagnostic.Source.StartHttpActivity("Get: /users");

            var response = await mediator.Send(GetUsersQuery.Instance);

            return Results.Ok(response);
        });


        group.MapGet("{id:guid}", async (IMediator mediator, ILoggerFactory loggerFactory, Guid id) =>
        {
            Diagnostic.AddHttpRequest();

            using var activity = Diagnostic.Source
                .StartHttpActivity("Get: /users/{id}")?
                .SetTag("UserId", id.ToString());

            var response = await mediator.Send(new GetUserQuery(id));
            return Results.Ok(response);
        }).WithName("GetUser");


        group.MapGet("{id:guid}/total-notifications", async (IMediator mediator, ILoggerFactory loggerFactory, Guid id) =>
        {
            Diagnostic.AddHttpRequest();

            using var activity = Diagnostic.Source
                .StartHttpActivity("Get: /users/{id}/total-notifications")?
                .SetTag("UserId", id.ToString());

            var response = await mediator.Send(new GetUserNotificationsTotals(id));
            return Results.Ok(response);
        });


        group.MapPost("", async (IMediator mediator, ILoggerFactory loggerFactory, UserRequest request) =>
        {
            Diagnostic.AddHttpRequest();

            using var activity = Diagnostic.Source.StartHttpActivity("Post: /users");

            var id = await mediator.Send(new CreateUserCommand(request));

            activity?.SetTag("UserId", id.ToString());

            return Results.CreatedAtRoute(
                "GetUser",
                new { id },
                id);
        });


        group.MapPut("{id:guid}", async (IMediator mediator, ILoggerFactory loggerFactory, Guid id, UserRequest request) =>
        {
            Diagnostic.AddHttpRequest();

            using var activity = Diagnostic.Source
                .StartHttpActivity("Put: /users")?
                .SetTag("UserId", id.ToString());

            await mediator.Send(new UpdateUserCommand(id, request));

            return Results.NoContent();
        });


        group.MapDelete("{id:guid}", async (IMediator mediator, ILoggerFactory loggerFactory, Guid id) =>
        {
            Diagnostic.AddHttpRequest();

            using var activity = Diagnostic.Source
                .StartHttpActivity("Delete: /users")?
                .SetTag("UserId", id.ToString());

            await mediator.Send(new DeleteUserCommand(id));

            return Results.NoContent();
        });
    }
}
