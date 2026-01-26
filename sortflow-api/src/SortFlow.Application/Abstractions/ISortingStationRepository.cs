using SortFlow.Domain.Entities;

namespace SortFlow.Application.Abstractions;

public interface ISortingStationRepository
{
    Task<IReadOnlyList<SortingStation>> GetAllAsync(CancellationToken cancellationToken);
    Task<SortingStation?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<SortingStation>> GetByZoneIdAsync(Guid zoneId, CancellationToken cancellationToken);
    Task AddAsync(SortingStation station, CancellationToken cancellationToken);
    Task UpdateAsync(SortingStation station, CancellationToken cancellationToken);
    Task DeleteAsync(SortingStation station, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<SortingStation> stations, CancellationToken cancellationToken);
}
