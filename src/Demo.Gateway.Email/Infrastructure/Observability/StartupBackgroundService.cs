using Microsoft.Extensions.Diagnostics.HealthChecks;
using static Gateway.Email.Infrastructure.Observability.StartupBackgroundService;

namespace Gateway.Email.Infrastructure.Observability;

public sealed class StartupBackgroundService(
    ILogger<StartupBackgroundService> logger,
    HealthCheck healthCheck,
    IEnumerable<IStartup> startups) : BackgroundService
{
    private readonly ILogger<StartupBackgroundService> _logger = logger;
    private readonly HealthCheck _healthCheck = healthCheck;
    private readonly IEnumerable<IStartup> _startups = startups;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[INFRASTRUCTURE][Startup] Starting...");

        await Task.WhenAll(_startups.Select(s => s.ExecuteAsync(stoppingToken)));

        _healthCheck.StartupCompleted = true;

        _logger.LogInformation("[INFRASTRUCTURE][Startup] Ended");
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
