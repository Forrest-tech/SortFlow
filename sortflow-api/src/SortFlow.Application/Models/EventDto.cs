namespace SortFlow.Application.Models;

public sealed class EventDto
{
    public Guid Id { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public DateTimeOffset ProcessedAtUtc { get; set; }
    public bool IsSuccessful { get; set; }
    public string? ExceptionType { get; set; }
    public string StationName { get; set; } = string.Empty;
    public string ZoneName { get; set; } = string.Empty;
}
