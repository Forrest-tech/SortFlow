namespace SortFlow.Domain.Entities;

public class SortingEvent
{
    public Guid Id { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public DateTimeOffset ProcessedAt { get; set; }
    public bool IsSuccessful { get; set; }
    public Guid SortingStationId { get; set; }
    public SortingStation? SortingStation { get; set; }
    public Guid ZoneId { get; set; }
    public Zone? Zone { get; set; }
    public SortingException? SortingException { get; set; }
}
