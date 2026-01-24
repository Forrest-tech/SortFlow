using Microsoft.EntityFrameworkCore;
using SortFlow.Application.Abstractions;
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
        return _dbContext.SortingEvents.CountAsync(e => e.ProcessedAt >= sinceUtc, cancellationToken);
    }

    public Task<int> CountSuccessfulSinceAsync(DateTimeOffset sinceUtc, CancellationToken cancellationToken)
    {
        return _dbContext.SortingEvents.CountAsync(e => e.ProcessedAt >= sinceUtc && e.IsSuccessful, cancellationToken);
    }
}
