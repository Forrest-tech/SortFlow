using SortFlow.Application.Models;

namespace SortFlow.Application.Abstractions;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(int? windowMinutes, DateTimeOffset? timeFrom, DateTimeOffset? timeTo, CancellationToken ct);
}
