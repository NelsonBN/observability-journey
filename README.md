# Demo about a journey to Observability


## Components
* [Instrumentation](./docs/Components.Instrumentation.md)

* [Grafana](./docs/Components.Grafana.md)
* [Prometheus](./docs/Components.Prometheus.md)
* [Loki](./docs/Components.Loki.md)
* [Tempo](./docs/Components.Tempo.md)
* [Pyroscope](./docs/Components.Pyroscope.md)
* [Alertmanager](./docs/Components.Alertmanager.md)
* [OpenTelemetry Collector](./docs/Components.OpenTelemetryCollector.md)

* [Exporters](./docs/Components.Exporters.md)



## Services
* [RabbitMQ](./docs/Services.RabbitMQ.md)



## How can use it?


### Build Images of Deemo projects

In the root of the project, run the following command:

```bash
docker build -t <image-name> -f <dockerfile> .
```

#### `Api.Users`

```bash
docker build -t api-users -f ./src/Demo.Api.Users/Dockerfile .
```


#### `Api.Notifications`

```bash
docker build -t api-notifications -f ./src/Demo.Api.Notifications/Dockerfile .
```

#### `Gateway.Email`

```bash
docker build -t gateway-email -f ./src/Demo.Gateway.Email/Dockerfile .
```

#### `Gateway.SMS`

```bash
docker build -t gateway-sms -f ./src/Demo.Gateway.SMS/Dockerfile .
```




### Run project

#### Only external services

```bash
docker compose up mongo postgres redis rabbitmq prometheus tempo pyroscope postgres-exporter grafana pgweb mongo-express redis-commander mailpit
```


#### Just external services

```bash
docker compose up mongo postgres redis rabbitmq
```

#### Minimous external services

```bash
docker compose up mongo postgres redis rabbitmq prometheus loki tempo pyroscope otel-collector
```

#### Just Observability services

```bash
docker compose up prometheus loki tempo pyroscope otel-collector grafana
```

#### Full project

```bash
docker compose up --build
## or
docker compose up --build mongo postgres redis rabbitmq prometheus loki tempo pyroscope postgres-exporter otel-collector grafana pgweb mongo-express redis-commander mailpit api-users api-notifications gateway-email gateway-sms
```



### Run tests

```bash
k6 run ./tests/test.js --vus 15 --duration 120s
```
