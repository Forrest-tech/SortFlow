namespace SortFlow.Application.Models;

public sealed class ZoneDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int StationCount { get; set; }
}
