using System;
using System.Threading;
using Api.Users.DTOs;
using Api.Users.UseCases;
using BuildingBlocks.Observability;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Api.Users.Infrastructure.Http;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/users")
            .WithOpenApi();


        group.MapGet("", async (GetUsersQuery query, CancellationToken cancellationToken) =>
        {
            Telemetry.AddHttpRequest();

            using var activity = Telemetry.Source.StartHttpActivity("Get: /users");

            var response = await query.HandleAsync(cancellationToken);

            return Results.Ok(response);
        });


        group.MapGet("{id:guid}", async (GetUserQuery query, Guid id, CancellationToken cancellationToken) =>
        {
            Telemetry.AddHttpRequest();

            using var activity = Telemetry.Source
                .StartHttpActivity("Get: /users/{id}")?
                .SetTag("UserId", id.ToString());

            var response = await query.HandleAsync(id, cancellationToken);
            return Results.Ok(response);
        }).WithName("GetUser");


        group.MapGet("{id:guid}/total-notifications", async (GetUserNotificationsTotalsQuery query, Guid id, CancellationToken cancellationToken) =>
        {
            Telemetry.AddHttpRequest();

            using var activity = Telemetry.Source
                .StartHttpActivity("Get: /users/{id}/total-notifications")?
                .SetTag("UserId", id.ToString());

            var response = await query.HandleAsync(id, cancellationToken);
            return Results.Ok(response);
        });


        group.MapPost("", async (CreateUserCommand command, UserRequest request, CancellationToken cancellationToken) =>
        {
            Telemetry.AddHttpRequest();

            using var activity = Telemetry.Source.StartHttpActivity("Post: /users");

            var id = await command.HandleAsync(request, cancellationToken);

            activity?.SetTag("UserId", id.ToString());

            return Results.CreatedAtRoute(
                "GetUser",
                new { id },
                id);
        });


        group.MapPut("{id:guid}", async (UpdateUserCommand command, Guid id, UserRequest request, CancellationToken cancellationToken) =>
        {
            Telemetry.AddHttpRequest();

            using var activity = Telemetry.Source
                .StartHttpActivity("Put: /users")?
                .SetTag("UserId", id.ToString());

            await command.HandleAsync(id, request, cancellationToken);

            return Results.NoContent();
        });


        group.MapDelete("{id:guid}", async (DeleteUserCommand command, Guid id, CancellationToken cancellationToken) =>
        {
            Telemetry.AddHttpRequest();

            using var activity = Telemetry.Source
                .StartHttpActivity("Delete: /users")?
                .SetTag("UserId", id.ToString());

            await command.HandleAsync(id, cancellationToken);

            return Results.NoContent();
        });
    }
}
