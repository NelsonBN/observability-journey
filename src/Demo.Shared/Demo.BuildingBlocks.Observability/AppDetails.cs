using System.Reflection;

namespace BuildingBlocks.Observability;

public static class AppDetails
{
    public static string Name => Assembly.GetEntryAssembly()!.GetName().Name!; // TODO: try to remove this
    public static string Version => Assembly.GetEntryAssembly()!.GetName().Version!.ToString() ?? "Unknown"; // TODO: try to remove this
}
