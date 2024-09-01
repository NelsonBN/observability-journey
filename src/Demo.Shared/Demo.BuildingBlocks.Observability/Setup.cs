//using HealthChecks.UI.Client;
//using Microsoft.AspNetCore.Diagnostics.HealthChecks;

//namespace BuildingBlocks.Observability;

//public static class Setup
//{
//    public static IApplicationBuilder AddHealthChecks(this IApplicationBuilder app)
//    {
//        app.UseEndpoints(endpoints => endpoints.MapHealthChecks("/healthz/startup", new HealthCheckOptions
//        {
//            Predicate = check => check.Tags.Contains("Startup"),
//            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//        }));

//        app.UseEndpoints(endpoints => endpoints.MapHealthChecks("/healthz/live", new HealthCheckOptions
//        {
//            Predicate = _ => false,
//            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//        }));

//        app.UseEndpoints(endpoints => endpoints.MapHealthChecks("/healthz/ready", new HealthCheckOptions
//        {
//            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//        }));

//        return app;
//    }
//}
// TODO: to finish
