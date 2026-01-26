using SortFlow.Domain.Enums;

namespace SortFlow.Domain.Entities;

public class SortingEvent
{
    public Guid Id { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public Guid StationId { get; set; }
    public Guid ZoneId { get; set; }
    /// <summary>OK or ExceptionType name</summary>
    public string Result { get; set; } = "OK";
    public DateTimeOffset Timestamp { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public bool IsSuccessful => string.Equals(Result, "OK", StringComparison.OrdinalIgnoreCase);

    public SortingStation? SortingStation { get; set; }
    public Zone? Zone { get; set; }
    public SortingException? SortingException { get; set; }
}
