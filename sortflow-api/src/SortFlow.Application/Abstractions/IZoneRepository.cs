using SortFlow.Domain.Entities;

namespace SortFlow.Application.Abstractions;

public interface IZoneRepository
{
    Task<IReadOnlyList<Zone>> GetAllAsync(CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<Zone> zones, CancellationToken cancellationToken);
}
