namespace SortFlow.Application.Models;

public sealed class ExceptionDto
{
    public Guid Id { get; set; }
    public string ExceptionType { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
}
