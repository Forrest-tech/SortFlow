using SortFlow.Domain.Entities;

namespace SortFlow.Application.Abstractions;

public interface IZoneRepository
{
    Task<IReadOnlyList<Zone>> GetAllAsync(CancellationToken cancellationToken);
    Task<Zone?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Zone zone, CancellationToken cancellationToken);
    Task UpdateAsync(Zone zone, CancellationToken cancellationToken);
    Task DeleteAsync(Zone zone, CancellationToken cancellationToken);
    Task<int> CountStationsInZoneAsync(Guid zoneId, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<Zone> zones, CancellationToken cancellationToken);
}
