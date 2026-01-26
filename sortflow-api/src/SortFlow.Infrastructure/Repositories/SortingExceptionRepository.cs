using Microsoft.EntityFrameworkCore;
using SortFlow.Application.Abstractions;
using SortFlow.Application.Models;
using SortFlow.Domain.Entities;
using SortFlow.Domain.Enums;
using SortFlow.Infrastructure.Data;

namespace SortFlow.Infrastructure.Repositories;

public class SortingExceptionRepository : ISortingExceptionRepository
{
    private readonly SortFlowDbContext _dbContext;

    public SortingExceptionRepository(SortFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(SortingException sortingException, CancellationToken cancellationToken)
    {
        _dbContext.SortingExceptions.Add(sortingException);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<int> CountSinceAsync(DateTimeOffset sinceUtc, CancellationToken cancellationToken)
    {
        return _dbContext.SortingExceptions.CountAsync(e => e.Timestamp >= sinceUtc, cancellationToken);
    }

    public Task<int> CountByDateRangeAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken)
    {
        return _dbContext.SortingExceptions.CountAsync(e => e.Timestamp >= from && e.Timestamp < to, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<ExceptionType, int>> CountByTypeAndDateRangeAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken)
    {
        var list = await _dbContext.SortingExceptions
            .Where(e => e.Timestamp >= from && e.Timestamp < to)
            .GroupBy(e => e.ExceptionType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);
        return list.ToDictionary(x => x.Type, x => x.Count);
    }

    public async Task<IReadOnlyList<SortingException>> GetRecentAsync(int limit, CancellationToken cancellationToken)
    {
        return await _dbContext.SortingExceptions
            .Include(e => e.SortingEvent)
            .ThenInclude(ev => ev!.SortingStation)
            .OrderByDescending(e => e.Timestamp)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResultDto<SortingException>> GetPagedAsync(ExceptionsFilterDto filter, CancellationToken cancellationToken)
    {
        var q = _dbContext.SortingExceptions
            .Include(e => e.SortingEvent)
            .ThenInclude(ev => ev!.SortingStation)
            .AsQueryable();

        if (filter.ZoneId.HasValue)
            q = q.Where(e => e.SortingEvent != null && e.SortingEvent.ZoneId == filter.ZoneId.Value);
        if (filter.StationId.HasValue)
            q = q.Where(e => e.SortingEvent != null && e.SortingEvent.StationId == filter.StationId.Value);
        if (filter.TimeFrom.HasValue)
            q = q.Where(e => e.Timestamp >= filter.TimeFrom.Value);
        if (filter.TimeTo.HasValue)
            q = q.Where(e => e.Timestamp < filter.TimeTo.Value);
        if (!string.IsNullOrEmpty(filter.ExceptionType))
            q = q.Where(e => e.ExceptionType.ToString() == filter.ExceptionType);

        var total = await q.CountAsync(cancellationToken);

        var sortBy = (filter.SortBy ?? "Timestamp").ToLowerInvariant();
        var asc = string.Equals(filter.SortDir, "asc", StringComparison.OrdinalIgnoreCase);
        q = sortBy switch
        {
            "exceptiontype" => asc ? q.OrderBy(e => e.ExceptionType) : q.OrderByDescending(e => e.ExceptionType),
            "details" => asc ? q.OrderBy(e => e.Details) : q.OrderByDescending(e => e.Details),
            _ => asc ? q.OrderBy(e => e.Timestamp) : q.OrderByDescending(e => e.Timestamp)
        };

        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 200);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResultDto<SortingException> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }
}
