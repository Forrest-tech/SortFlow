using SortFlow.Application.Models;
using SortFlow.Domain.Entities;
using SortFlow.Domain.Enums;

namespace SortFlow.Application.Abstractions;

public interface ISortingExceptionRepository
{
    Task AddAsync(SortingException sortingException, CancellationToken cancellationToken);
    Task<int> CountSinceAsync(DateTimeOffset sinceUtc, CancellationToken cancellationToken);
    Task<int> CountByDateRangeAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken);
    Task<IReadOnlyDictionary<ExceptionType, int>> CountByTypeAndDateRangeAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken);
    Task<IReadOnlyList<SortingException>> GetRecentAsync(int limit, CancellationToken cancellationToken);
    Task<PagedResultDto<SortingException>> GetPagedAsync(ExceptionsFilterDto filter, CancellationToken cancellationToken);
}
