namespace SortFlow.Application.Models;

public sealed class DashboardSummaryDto
{
    public double ItemsPerMinute { get; set; }
    public int ItemsPerHour { get; set; }
    public int TotalToday { get; set; }
    public double SuccessRate { get; set; }
    public int TotalEventsLastHour { get; set; }
    public int SuccessfulEventsLastHour { get; set; }
    public int ExceptionsLastHour { get; set; }
    public Dictionary<string, int>? EventsByCategory { get; set; }
    public DateTimeOffset GeneratedAtUtc { get; set; }
}
