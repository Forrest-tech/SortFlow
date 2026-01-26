using SortFlow.Application.Models;

namespace SortFlow.Application.Abstractions;

public interface IHistoryService
{
    Task<IReadOnlyList<HistoryDto>> GetAggregatedAsync(string groupBy, DateTimeOffset from, DateTimeOffset to, CancellationToken ct);
    Task<byte[]> GetExportCsvAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken ct);
}
