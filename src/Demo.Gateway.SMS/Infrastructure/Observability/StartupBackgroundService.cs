using Microsoft.Extensions.Diagnostics.HealthChecks;
using static Gateway.SMS.Infrastructure.Observability.StartupBackgroundService;

namespace Gateway.SMS.Infrastructure.Observability;

public sealed class StartupBackgroundService(
    ILogger<StartupBackgroundService> logger,
    HealthCheck healthCheck) : BackgroundService
{
    private readonly ILogger<StartupBackgroundService> _logger = logger;
    private readonly HealthCheck _healthCheck = healthCheck;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[INFRASTRUCTURE][Startup] Starting...");

        _healthCheck.StartupCompleted = true;

        _logger.LogInformation("[INFRASTRUCTURE][Startup] Ended");

        await Task.CompletedTask;
    }



    public sealed class HealthCheck : IHealthCheck
    {
        private volatile bool _isReady;

        public bool StartupCompleted
        {
            set => _isReady = value;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            => Task.FromResult(
                _isReady ?
                HealthCheckResult.Healthy("The startup has completed") :
                HealthCheckResult.Unhealthy("That startup is still running"));
    }
}
