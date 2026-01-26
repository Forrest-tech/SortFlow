using SortFlow.Application.Models;

namespace SortFlow.Application.Abstractions;

public interface IZoneService
{
    Task<IReadOnlyList<ZoneDto>> GetAllAsync(CancellationToken ct);
    Task<ZoneDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<ZoneDto> CreateAsync(ZoneDto dto, CancellationToken ct);
    Task<ZoneDto?> UpdateAsync(Guid id, ZoneDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}
