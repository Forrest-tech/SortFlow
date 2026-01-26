using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SortFlow.Application.Abstractions;
using SortFlow.Application.Models;
using SortFlow.Infrastructure.Data;

namespace SortFlow.Infrastructure.Repositories;

public class HistoryRepository : IHistoryRepository
{
    private readonly SortFlowDbContext _dbContext;

    public HistoryRepository(SortFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<HistoryDto>> GetDailyAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken ct)
    {
        var toExclusive = to.AddDays(1);
        var events = await _dbContext.SortingEvents
            .Where(e => e.Timestamp >= from && e.Timestamp < toExclusive)
            .ToListAsync(ct);

        var grouped = events
            .GroupBy(e => e.Timestamp.UtcDateTime.Date)
            .Select(g => new HistoryDto
            {
                Period = g.Key.ToString("yyyy-MM-dd"),
                Total = g.Count(),
                Successful = g.Count(e => e.Result == "OK"),
                Exceptions = g.Count(e => e.Result != "OK"),
                SuccessRate = g.Count() > 0 ? Math.Round(100.0 * g.Count(e => e.Result == "OK") / g.Count(), 2) : 0
            })
            .OrderBy(x => x.Period)
            .ToList();

        return grouped;
    }

    public async Task<IReadOnlyList<HistoryDto>> GetWeeklyAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken ct)
    {
        var toExclusive = to.AddDays(1);
        var events = await _dbContext.SortingEvents
            .Where(e => e.Timestamp >= from && e.Timestamp < toExclusive)
            .ToListAsync(ct);

        var year = from.UtcDateTime.Year;
        var grouped = events
            .GroupBy(e => System.Globalization.ISOWeek.GetWeekOfYear(e.Timestamp.UtcDateTime))
            .Select(g => new
            {
                Week = g.Key,
                Total = g.Count(),
                Successful = g.Count(e => e.Result == "OK"),
                Exceptions = g.Count(e => e.Result != "OK")
            });

        return grouped.Select(x => new HistoryDto
        {
            Period = $"{year}-W{x.Week:D2}",
            Total = x.Total,
            Successful = x.Successful,
            Exceptions = x.Exceptions,
            SuccessRate = x.Total > 0 ? Math.Round(100.0 * x.Successful / x.Total, 2) : 0
        }).ToList();
    }

    public async Task<IReadOnlyList<HistoryDto>> GetMonthlyAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken ct)
    {
        var toExclusive = to.AddDays(1);
        var events = await _dbContext.SortingEvents
            .Where(e => e.Timestamp >= from && e.Timestamp < toExclusive)
            .ToListAsync(ct);

        var grouped = events
            .GroupBy(e => new { e.Timestamp.UtcDateTime.Year, e.Timestamp.UtcDateTime.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Total = g.Count(),
                Successful = g.Count(e => e.Result == "OK"),
                Exceptions = g.Count(e => e.Result != "OK")
            });

        return grouped.Select(x => new HistoryDto
        {
            Period = $"{x.Year}-{x.Month:D2}",
            Total = x.Total,
            Successful = x.Successful,
            Exceptions = x.Exceptions,
            SuccessRate = x.Total > 0 ? Math.Round(100.0 * x.Successful / x.Total, 2) : 0
        }).ToList();
    }
}
