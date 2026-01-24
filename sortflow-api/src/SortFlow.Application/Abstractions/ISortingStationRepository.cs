using SortFlow.Domain.Entities;

namespace SortFlow.Application.Abstractions;

public interface ISortingStationRepository
{
    Task<IReadOnlyList<SortingStation>> GetAllAsync(CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<SortingStation> stations, CancellationToken cancellationToken);
}
