FROM mcr.microsoft.com/dotnet/aspnet:9.0.3-alpine3.20-amd64 AS base-env
# Preparing the runtime environment

WORKDIR /app
EXPOSE 8080


HEALTHCHECK --interval=10s \
            --timeout=5s \
            --start-period=3s \
            --retries=5 \
    CMD wget --quiet --tries=1 --spider http://localhost:8080/healthz/ready || exit 1


### Configure pyroscope
ENV PYROSCOPE_APPLICATION_NAME=Api.Users
# Enable the Pyroscope profiler
ENV CORECLR_ENABLE_PROFILING=1
# The Pyroscope profiler GUID
ENV CORECLR_PROFILER={BD1A650D-AC5D-4896-B64F-D6FA25D6B26A}
# The Pyroscope profiler path.
ENV CORECLR_PROFILER_PATH=/app/Pyroscope.Profiler.Native.so
# The Pyroscope profiler configuration. The Pyroscope agent will listen
ENV LD_PRELOAD=/app/Pyroscope.Linux.ApiWrapper.x64.so

ENV DOTNET_EnableDiagnostics=1
ENV DOTNET_EnableDiagnostics_IPC=0
ENV DOTNET_EnableDiagnostics_Debugger=0
ENV DOTNET_EnableDiagnostics_Profiler=1

ENV PYROSCOPE_LOG_LEVEL=info
# Defaults false
ENV PYROSCOPE_PROFILING_ENABLED=true
ENV PYROSCOPE_PROFILING_ALLOCATION_ENABLED=true
ENV PYROSCOPE_PROFILING_CONTENTION_ENABLED=true
ENV PYROSCOPE_PROFILING_EXCEPTION_ENABLED=true
ENV PYROSCOPE_PROFILING_WALLTIME_ENABLED=true
ENV PYROSCOPE_PROFILING_LOCK_ENABLED=true
ENV PYROSCOPE_PROFILING_HEAP_ENABLED=true
# Defaults true
ENV PYROSCOPE_PROFILING_CPU_ENABLED=true

ENV PYROSCOPE_LABELS=system.environment:production

# Create the log directory and set the appropriate permissions
RUN mkdir -p /var/log/pyroscope/dotnet && \
    chown -R app:app /var/log/pyroscope && \
    chmod -R 750 /var/log/pyroscope


# Install the required dependencies to handle the internationalization
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

RUN apk add --no-cache \
            icu-libs \
            icu-data-full \
            tzdata

RUN apk upgrade musl

USER app

# Skip extraction of XML docs - generally not useful within an image/container - helps performance
ENV NUGET_XMLDOC_MODE=skip

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://*:8080
ENV ASPNETCORE_HTTP_PORTS=
ENV ASPNETCORE_HTTPS_PORTS=



FROM mcr.microsoft.com/dotnet/sdk:9.0.201 AS build-env

WORKDIR /libs

### Configure the pyroscope
RUN wget -qO- https://github.com/grafana/pyroscope-dotnet/releases/download/v0.9.4/pyroscope.0.9.4-musl-x86_64.tar.gz \
    | tar xvz -C .


WORKDIR /src

# Copy just the project files and restore the nuget packages
COPY ./src/Demo.Api.Users/*.csproj ./Demo.Api.Users/
COPY ./src/Demo.Shared/Demo.Contracts/*.csproj ./Demo.Shared/Demo.Contracts/
COPY ./src/Demo.Shared/Demo.BuildingBlocks.Observability/*.csproj ./Demo.Shared/Demo.BuildingBlocks.Observability/
COPY ./src/Demo.Shared/Demo.Contracts.Notifications/*.csproj ./Demo.Shared/Demo.Contracts.Notifications/

# Restore nuget packages
RUN dotnet restore ./Demo.Api.Users/*.csproj --runtime linux-musl-x64


# Copy all the source code and build
COPY ./src/Demo.Api.Users/ ./Demo.Api.Users/
COPY ./src/Demo.Shared/Demo.Contracts/ ./Demo.Shared/Demo.Contracts/
COPY ./src/Demo.Shared/Demo.BuildingBlocks.Observability/ ./Demo.Shared/Demo.BuildingBlocks.Observability/
COPY ./src/Demo.Shared/Demo.Contracts.Notifications/ ./Demo.Shared/Demo.Contracts.Notifications/


# Build and publish the application. Used the "--no-restore" and "--no-build" to benefit the layer caches
RUN dotnet build -c Release ./Demo.Api.Users/*.csproj --runtime linux-musl-x64

# We can use `--runtime linux-musl-x64` or `-r $(dotnet --info | grep RID | cut -b 7- | tr -d ' ')` to get the runtime identifier
RUN dotnet publish ./Demo.Api.Users/*.csproj \
    -c Release \
    --runtime linux-musl-x64 \
    --no-build \
    --no-restore \
    -o /publish



FROM base-env AS run-env

COPY --from=build-env /publish .
COPY --from=build-env /libs .

ENTRYPOINT ["dotnet", "Api.Users.dll"]
