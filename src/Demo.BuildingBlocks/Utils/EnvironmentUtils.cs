namespace Common.Utils;

public static class EnvironmentUtils
{
    public static string GetAspNetEnvironment()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if(string.IsNullOrWhiteSpace(environment))
        {
            return "Production";
        }
        return environment;
    }
}
