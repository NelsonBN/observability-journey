using System;
using Api.Users.Domain;
using BuildingBlocks.Contracts.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Users.Infrastructure.NotificationsApi;

public static class Setup
{
    public static IServiceCollection AddGrpcClient(this IServiceCollection services)
    {
        services
            .AddTransient<INotificationsService, NotificationsService>()
            .AddGrpcClient<Greeter.GreeterClient>((sp, options) =>
            {
                var uri = sp.GetRequiredService<IConfiguration>()["NotificationsApi"];
                options.Address = new Uri(uri!);
            });

        return services;
    }
}
