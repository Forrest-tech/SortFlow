namespace SortFlow.Domain.Entities;

public class SortingStation
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StationCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid ZoneId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Zone? Zone { get; set; }
    public ICollection<SortingEvent> SortingEvents { get; set; } = new List<SortingEvent>();
}
