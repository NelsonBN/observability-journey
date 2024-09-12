using System;
using System.Diagnostics;
using System.Text.Json;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Observability;

public static class TelemetryExtensions
{
    public static Activity? StartHttpActivity(this ActivitySource activitySource, string description)
        => activitySource.StartActivity(description, ActivityKind.Server);

    public static Activity? AddMessage<TMessage>(this Activity? activity, TMessage message)
        => activity?.SetTag("message", JsonSerializer.Serialize(message));

    public static Activity? RegisterException<TException>(this Activity? activity, TException exception)
        where TException : Exception
    {
        if(activity is null)
        {
            return activity;
        }

        activity.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity.RecordException(exception);

        return activity;
    }

    public static Activity? RegisterValidation<TValidation>(this Activity? activity, TValidation validation)
        where TValidation : Exception
    {
        if(activity is null)
        {
            return activity;
        }

        activity.AddEvent(new(
            typeof(TValidation).Name,
            tags: new([
                new("message", validation.Message)]
            )));

        return activity;
    }
}
