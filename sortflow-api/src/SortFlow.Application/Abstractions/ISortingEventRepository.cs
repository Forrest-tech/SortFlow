using SortFlow.Domain.Entities;

namespace SortFlow.Application.Abstractions;

public interface ISortingEventRepository
{
    Task AddAsync(SortingEvent sortingEvent, CancellationToken cancellationToken);
    Task<int> CountSinceAsync(DateTimeOffset sinceUtc, CancellationToken cancellationToken);
    Task<int> CountSuccessfulSinceAsync(DateTimeOffset sinceUtc, CancellationToken cancellationToken);
    Task<IReadOnlyList<SortingEvent>> GetRecentAsync(int limit, CancellationToken cancellationToken);
}
