using SortFlow.Domain.Entities;

namespace SortFlow.Application.Abstractions;

public interface ISortingExceptionRepository
{
    Task AddAsync(SortingException sortingException, CancellationToken cancellationToken);
    Task<int> CountSinceAsync(DateTimeOffset sinceUtc, CancellationToken cancellationToken);
    Task<IReadOnlyList<SortingException>> GetRecentAsync(int limit, CancellationToken cancellationToken);
}
