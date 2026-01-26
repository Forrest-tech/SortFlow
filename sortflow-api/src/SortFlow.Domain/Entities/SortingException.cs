using SortFlow.Domain.Enums;

namespace SortFlow.Domain.Entities;

public class SortingException
{
    public Guid Id { get; set; }
    public Guid SortingEventId { get; set; }
    public ExceptionType ExceptionType { get; set; }
    public string Details { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public SortingEvent? SortingEvent { get; set; }
}
