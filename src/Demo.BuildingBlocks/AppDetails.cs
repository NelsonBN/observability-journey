using System.Reflection;

namespace BuildingBlocks;

public static class AppDetails
{
    public static string Name => Assembly.GetEntryAssembly()!.GetName().Name!;
    public static string ServiceId => Environment.MachineName;
    public static string Version => Assembly.GetEntryAssembly()!.GetName().Version!.ToString() ?? "Unknown";
}
