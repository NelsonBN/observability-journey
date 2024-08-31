using Api.Notifications.Domain;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.Notifications.Infrastructure.Database;

public static class Setup
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
        => services
            .AddDbContext<DataContext>()
            .AddScoped<INotificationsRepository, NotificationsRepository>()
            .AddScoped<IReportsRepository, ReportsRepository>();

    public static IHealthChecksBuilder AddDatabase(this IHealthChecksBuilder builder)
        => builder.AddDbContextCheck<DataContext>(
            "EFCore",
            HealthStatus.Unhealthy);
}
