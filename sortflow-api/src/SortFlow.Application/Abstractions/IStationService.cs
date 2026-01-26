using SortFlow.Application.Models;

namespace SortFlow.Application.Abstractions;

public interface IStationService
{
    Task<IReadOnlyList<StationDto>> GetAllAsync(CancellationToken ct);
    Task<StationDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<StationDto> CreateAsync(StationDto dto, CancellationToken ct);
    Task<StationDto?> UpdateAsync(Guid id, StationDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}
