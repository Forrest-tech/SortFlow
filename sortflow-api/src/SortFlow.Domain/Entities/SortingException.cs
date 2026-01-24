using SortFlow.Domain.Enums;

namespace SortFlow.Domain.Entities;

public class SortingException
{
    public Guid Id { get; set; }
    public Guid SortingEventId { get; set; }
    public SortingEvent? SortingEvent { get; set; }
    public ExceptionType ExceptionType { get; set; }
    public string Details { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
