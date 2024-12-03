# Demo - Full Observability - Components - Grafana

[<< Back](../README.md)

## Dashboards

- [.NET team](https://grafana.com/orgs/dotnetteam)
  - [ASP.NET Core](https://grafana.com/grafana/dashboards/19924-asp-net-core/)
  - [ASP.NET Core Endpoint](https://grafana.com/grafana/dashboards/19925-asp-net-core-endpoint/)
- [ASP.NET OTEL Metrics](https://grafana.com/grafana/dashboards/17706-asp-net-otel-metrics/)
- **OpenTelemetry Collector**
  - [OpenTelemetry Collector](https://grafana.com/grafana/dashboards/12553-opentelemetry-collector/)
  - [OpenTelemetry Collector - Data Flow](https://grafana.com/grafana/dashboards/18309-opentelemetry-collector-data-flow/)
  - [OpenTelemetry Collector - v0.101.0](https://grafana.com/grafana/dashboards/15983-opentelemetry-collector/)
  - [OpenTelemetry Collector - v20230112](https://grafana.com/grafana/dashboards/17868-opentelemetry-collector-0-68-0-v20230112/)
  - [OpenTelemetry Collector - v20221229](https://grafana.com/grafana/dashboards/17728-opentelemetry-collector-0-68-0-v20221229/)

## Data Sources

- [Data Source documentation](https://grafana.com/docs/grafana/latest/administration/provisioning/#datasources)

### How can export datasources from Grafana?

```bash
curl --location 'http://localhost:3000/api/datasources' \
--header 'Content-Type: application/json'
```
