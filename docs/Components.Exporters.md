# Demo - Full Observability - Components - Exporters

[<< Back](../README.md)


## Data Exporters

### .NET Exporters

- [GitHub - OpenTelemetry .NET Exporters](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.OpenTelemetryProtocol/README.md)


#### Using Prometheus exporter in .NET

```csharp
<PackageReference Include="OpenTelemetry.Exporter.Prometheus.AspNetCore" Version="1.9.0-alpha.1" />
```
```csharp
public static IServiceCollection AddObservability(this IServiceCollection services)
{
    services
        .AddOpenTelemetry()
        .WithMetrics(options =>
            options.AddPrometheusExporter());
    return services;
}

public static IApplicationBuilder AddObservability(this IApplicationBuilder app)
{
    app.UseRouting();
    app.UseOpenTelemetryPrometheusScrapingEndpoint();

    return app;
}
```

### Postgres Exporter

- [Github Repository](https://github.com/prometheus-community/postgres_exporter)
- [Configuration](https://grafana.com/oss/prometheus/exporters/postgres-exporter/?tab=installation)
- [Grafana Dashboard](https://grafana.com/oss/prometheus/exporters/postgres-exporter/?tab=dashboards)
- [Alerts Rules](https://grafana.com/oss/prometheus/exporters/postgres-exporter/?tab=alerting-rules)


### Node Exporter

- [GitHub - Node Exporter](https://github.com/prometheus/node_exporter)
- [GitHub - Dashboard](https://github.com/rfmoz/grafana-dashboards)
- [Grafana Dashboard](https://grafana.com/grafana/dashboards/1860-node-exporter-full/)
