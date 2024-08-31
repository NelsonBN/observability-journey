namespace Api.Notifications.Domain;

public sealed record ReportState
{
    public int Id { get; init; } = 1;
    public required DateTime LastGeneratedAt { get; set; }
}
