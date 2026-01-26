using System.Globalization;
using System.Text;
using SortFlow.Application.Abstractions;
using SortFlow.Application.Models;

namespace SortFlow.Application.Services;

public class HistoryService(IHistoryRepository historyRepo, ISortingEventRepository eventRepo) : IHistoryService
{
    public async Task<IReadOnlyList<HistoryDto>> GetAggregatedAsync(string groupBy, DateTimeOffset from, DateTimeOffset to, CancellationToken ct)
    {
        return groupBy?.ToLowerInvariant() switch
        {
            "day" => await historyRepo.GetDailyAsync(from, to, ct),
            "week" => await historyRepo.GetWeeklyAsync(from, to, ct),
            "month" => await historyRepo.GetMonthlyAsync(from, to, ct),
            _ => await historyRepo.GetDailyAsync(from, to, ct)
        };
    }

    public async Task<byte[]> GetExportCsvAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken ct)
    {
        var filter = new EventsFilterDto { Page = 1, PageSize = 50_000, TimeFrom = from, TimeTo = to, SortBy = "Timestamp", SortDir = "asc" };
        var paged = await eventRepo.GetPagedAsync(filter, ct);
        var sb = new StringBuilder();
        sb.AppendLine("Timestamp,ItemId,PostalCode,StationId,ZoneId,Result");
        foreach (var e in paged.Items)
        {
            sb.AppendLine($"{e.Timestamp:O},{CsvEsc(e.ItemId)},{CsvEsc(e.PostalCode)},{e.StationId},{e.ZoneId},{CsvEsc(e.Result)}");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string CsvEsc(string s) => string.IsNullOrEmpty(s) ? "" : s.Contains(',') || s.Contains('"') ? $"\"{s.Replace("\"", "\"\"")}\"" : s;
}
