namespace SortFlow.Domain.Entities;

public class Zone
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<SortingStation> SortingStations { get; set; } = new List<SortingStation>();
    public ICollection<SortingEvent> SortingEvents { get; set; } = new List<SortingEvent>();
}
