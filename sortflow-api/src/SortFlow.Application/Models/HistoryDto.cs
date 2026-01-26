namespace SortFlow.Application.Models;

public sealed class HistoryDto
{
    public string Period { get; set; } = string.Empty; // "2025-01-26", "2025-W04", "2025-01"
    public int Total { get; set; }
    public int Successful { get; set; }
    public int Exceptions { get; set; }
    public double SuccessRate { get; set; }
}
