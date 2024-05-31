# Demo - Full Observability - Components - Grafana

[<< Back](../README.md)

## Dashboards

- [.NET team](https://grafana.com/orgs/dotnetteam)
  - [ASP.NET Core](https://grafana.com/grafana/dashboards/19924-asp-net-core/)
  - [ASP.NET Core Endpoint](https://grafana.com/grafana/dashboards/19925-asp-net-core-endpoint/)
- [ASP.NET OTEL Metrics](https://grafana.com/grafana/dashboards/17706-asp-net-otel-metrics/)

## Data Sources

- [Data Source documentation](https://grafana.com/docs/grafana/latest/administration/provisioning/#datasources)

### How can export datasources from Grafana?

```bash
curl --location 'http://localhost:3000/api/datasources' \
--header 'Content-Type: application/json'
```
