using Microsoft.EntityFrameworkCore;
using SortFlow.Application.Abstractions;
using SortFlow.Application.Models;
using SortFlow.Domain.Entities;
using SortFlow.Infrastructure.Data;

namespace SortFlow.Infrastructure.Repositories;

public class SortingEventRepository : ISortingEventRepository
{
    private readonly SortFlowDbContext _dbContext;

    public SortingEventRepository(SortFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(SortingEvent sortingEvent, CancellationToken cancellationToken)
    {
        _dbContext.SortingEvents.Add(sortingEvent);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<int> CountSinceAsync(DateTimeOffset sinceUtc, CancellationToken cancellationToken)
    {
        return _dbContext.SortingEvents.CountAsync(e => e.Timestamp >= sinceUtc, cancellationToken);
    }

    public Task<int> CountSuccessfulSinceAsync(DateTimeOffset sinceUtc, CancellationToken cancellationToken)
    {
        return _dbContext.SortingEvents.CountAsync(e => e.Timestamp >= sinceUtc && e.Result == "OK", cancellationToken);
    }

    public Task<int> CountByDateRangeAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken)
    {
        return _dbContext.SortingEvents.CountAsync(e => e.Timestamp >= from && e.Timestamp < to, cancellationToken);
    }

    public Task<int> CountSuccessfulByDateRangeAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken)
    {
        return _dbContext.SortingEvents.CountAsync(e => e.Timestamp >= from && e.Timestamp < to && e.Result == "OK", cancellationToken);
    }

    public async Task<IReadOnlyList<SortingEvent>> GetRecentAsync(int limit, CancellationToken cancellationToken)
    {
        return await _dbContext.SortingEvents
            .Include(e => e.SortingStation)
            .ThenInclude(s => s!.Zone)
            .Include(e => e.SortingException)
            .OrderByDescending(e => e.Timestamp)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResultDto<SortingEvent>> GetPagedAsync(EventsFilterDto filter, CancellationToken cancellationToken)
    {
        var q = _dbContext.SortingEvents
            .Include(e => e.SortingStation)
            .ThenInclude(s => s!.Zone)
            .Include(e => e.SortingException)
            .AsQueryable();

        if (filter.ZoneId.HasValue)
            q = q.Where(e => e.ZoneId == filter.ZoneId.Value);
        if (filter.StationId.HasValue)
            q = q.Where(e => e.StationId == filter.StationId.Value);
        if (filter.TimeFrom.HasValue)
            q = q.Where(e => e.Timestamp >= filter.TimeFrom.Value);
        if (filter.TimeTo.HasValue)
            q = q.Where(e => e.Timestamp < filter.TimeTo.Value);
        if (!string.IsNullOrEmpty(filter.ExceptionType))
            q = q.Where(e => e.SortingException != null && e.SortingException.ExceptionType.ToString() == filter.ExceptionType);
        if (!string.IsNullOrEmpty(filter.Result))
        {
            if (string.Equals(filter.Result, "OK", StringComparison.OrdinalIgnoreCase))
                q = q.Where(e => e.Result == "OK");
            else
                q = q.Where(e => e.Result == filter.Result);
        }

        var total = await q.CountAsync(cancellationToken);

        var sortBy = (filter.SortBy ?? "Timestamp").ToLowerInvariant();
        var asc = string.Equals(filter.SortDir, "asc", StringComparison.OrdinalIgnoreCase);
        q = sortBy switch
        {
            "itemid" => asc ? q.OrderBy(e => e.ItemId) : q.OrderByDescending(e => e.ItemId),
            "postalcode" => asc ? q.OrderBy(e => e.PostalCode) : q.OrderByDescending(e => e.PostalCode),
            "result" => asc ? q.OrderBy(e => e.Result) : q.OrderByDescending(e => e.Result),
            "zoneid" => asc ? q.OrderBy(e => e.ZoneId) : q.OrderByDescending(e => e.ZoneId),
            "stationid" => asc ? q.OrderBy(e => e.StationId) : q.OrderByDescending(e => e.StationId),
            _ => asc ? q.OrderBy(e => e.Timestamp) : q.OrderByDescending(e => e.Timestamp)
        };

        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 200);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResultDto<SortingEvent> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }
}
