namespace SortFlow.Application.Models;

public sealed class StationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StationCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string ZoneName { get; set; } = string.Empty;
    public Guid ZoneId { get; set; }
}
