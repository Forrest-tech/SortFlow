using SortFlow.Application.Models;
using SortFlow.Domain.Entities;

namespace SortFlow.Application.Abstractions;

public interface ISortingEventRepository
{
    Task AddAsync(SortingEvent sortingEvent, CancellationToken cancellationToken);
    Task<int> CountSinceAsync(DateTimeOffset sinceUtc, CancellationToken cancellationToken);
    Task<int> CountSuccessfulSinceAsync(DateTimeOffset sinceUtc, CancellationToken cancellationToken);
    Task<int> CountByDateRangeAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken);
    Task<int> CountSuccessfulByDateRangeAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken);
    Task<IReadOnlyList<SortingEvent>> GetRecentAsync(int limit, CancellationToken cancellationToken);
    Task<PagedResultDto<SortingEvent>> GetPagedAsync(EventsFilterDto filter, CancellationToken cancellationToken);
}
