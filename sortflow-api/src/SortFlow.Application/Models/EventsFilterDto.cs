namespace SortFlow.Application.Models;

public sealed class EventsFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; }
    public string? SortDir { get; set; } // "asc" | "desc"
    public Guid? ZoneId { get; set; }
    public Guid? StationId { get; set; }
    public DateTimeOffset? TimeFrom { get; set; }
    public DateTimeOffset? TimeTo { get; set; }
    public string? ExceptionType { get; set; }
    public string? Result { get; set; } // "OK" or exception type name
}
