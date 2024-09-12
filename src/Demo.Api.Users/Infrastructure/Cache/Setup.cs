using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Api.Users.Infrastructure.Cache;

public static class Setup
{
    public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionMultiplexer = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!);
        services
            .AddSingleton(connectionMultiplexer)
            .AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = Assembly.GetEntryAssembly()!.GetName().Name;
                options.ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(connectionMultiplexer);
            });

        return services;
    }
}
