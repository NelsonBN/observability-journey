using Common;
using StackExchange.Redis;

namespace Api.Notifications.Infrastructure.Cache;

public static class Setup
{
    public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSingleton<Task<IConnectionMultiplexer>>(async (sp) =>
            {
                var connectionMultiplexer = configuration.GetConnectionString("Redis")!;
                return await ConnectionMultiplexer.ConnectAsync(connectionMultiplexer);
            })
            .AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var connectionMultiplexer = configuration.GetConnectionString("Redis")!;
                return ConnectionMultiplexer.Connect(connectionMultiplexer);
            });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis")!;
            options.InstanceName = AppDetails.Name;
        });

        return services;
    }
}
