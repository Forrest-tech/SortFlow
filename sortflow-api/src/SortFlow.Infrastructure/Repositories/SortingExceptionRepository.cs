using Microsoft.EntityFrameworkCore;
using SortFlow.Application.Abstractions;
using SortFlow.Domain.Entities;
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
        return _dbContext.SortingExceptions.CountAsync(e => e.CreatedAt >= sinceUtc, cancellationToken);
    }

    public async Task<IReadOnlyList<SortingException>> GetRecentAsync(int limit, CancellationToken cancellationToken)
    {
        return await _dbContext.SortingExceptions
            .Include(e => e.SortingEvent)
            .ThenInclude(e => e.SortingStation)
            .OrderByDescending(e => e.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
