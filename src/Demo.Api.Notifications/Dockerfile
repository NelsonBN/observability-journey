FROM mcr.microsoft.com/dotnet/aspnet:8.0.8-alpine3.20-amd64 AS base-env
# Preparing the runtime environment

WORKDIR /app
EXPOSE 8080
EXPOSE 8081


HEALTHCHECK --interval=10s \
            --timeout=5s \
            --start-period=3s \
            --retries=5 \
    CMD wget --quiet --tries=1 --spider http://localhost:8080/health || exit 1


### Configure pyroscope
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
ENV PYROSCOPE_PROFILING_ENABLED=1
ENV PYROSCOPE_PROFILING_ALLOCATION_ENABLED=true
ENV PYROSCOPE_PROFILING_CONTENTION_ENABLED=true
ENV PYROSCOPE_PROFILING_EXCEPTION_ENABLED=true

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

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://*:8080
ENV ASPNETCORE_HTTP_PORTS=
ENV ASPNETCORE_HTTPS_PORTS=



FROM mcr.microsoft.com/dotnet/sdk:8.0.401 AS build-env

WORKDIR /libs

### Configure the pyroscope
RUN wget -qO- https://github.com/grafana/pyroscope-dotnet/releases/download/v0.8.14-pyroscope/pyroscope.0.8.14-musl-x86_64.tar.gz \
    | tar xvz


WORKDIR /src

# Copy just the project files and restore the nuget packages
COPY ./src/Demo.Api.Notifications/*.csproj ./Demo.Api.Notifications/
COPY ./src/Demo.Common/*.csproj ./Demo.Common/

# Restore nuget packages
RUN dotnet restore ./Demo.Api.Notifications/*.csproj --runtime linux-musl-x64


# Copy all the source code and build
COPY ./src/Demo.Api.Notifications/ ./Demo.Api.Notifications/
COPY ./src/Demo.Common/ ./Demo.Common/


# Build and publish the application. Used the "--no-restore" and "--no-build" to benefit the layer caches
RUN dotnet build -c Release ./Demo.Api.Notifications/*.csproj --runtime linux-musl-x64

# We can use `--runtime linux-musl-x64` or `-r $(dotnet --info | grep RID | cut -b 7- | tr -d ' ')` to get the runtime identifier
RUN dotnet publish ./Demo.Api.Notifications/*.csproj \
    -c Release \
    --runtime linux-musl-x64 \
    --no-build \
    --no-restore \
    -o /publish



FROM base-env AS run-env

COPY --from=build-env /publish .
COPY --from=build-env /libs .

ENTRYPOINT ["dotnet", "Api.Notifications.dll"]
