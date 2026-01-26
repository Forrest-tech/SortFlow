using SortFlow.Application.Models;

namespace SortFlow.Application.Abstractions;

public interface IHistoryRepository
{
    Task<IReadOnlyList<HistoryDto>> GetDailyAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken ct);
    Task<IReadOnlyList<HistoryDto>> GetWeeklyAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken ct);
    Task<IReadOnlyList<HistoryDto>> GetMonthlyAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken ct);
}
