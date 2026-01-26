namespace SortFlow.Application.Models;

public sealed class ExceptionsFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public string? SortBy { get; set; }
    public string? SortDir { get; set; }
    public Guid? ZoneId { get; set; }
    public Guid? StationId { get; set; }
    public DateTimeOffset? TimeFrom { get; set; }
    public DateTimeOffset? TimeTo { get; set; }
    public string? ExceptionType { get; set; }
}
