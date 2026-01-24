namespace SortFlow.Application.Models;

public sealed class DashboardSummaryDto
{
    public int TotalEventsLastHour { get; set; }
    public int SuccessfulEventsLastHour { get; set; }
    public int ExceptionsLastHour { get; set; }
    public double ItemsPerMinute { get; set; }
    public double ItemsPerHour { get; set; }
    public DateTimeOffset GeneratedAtUtc { get; set; }
}
