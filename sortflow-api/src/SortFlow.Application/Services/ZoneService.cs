using SortFlow.Application.Abstractions;
using SortFlow.Application.Models;
using SortFlow.Domain.Entities;

namespace SortFlow.Application.Services;

public class ZoneService(IZoneRepository zoneRepo) : IZoneService
{
    public async Task<IReadOnlyList<ZoneDto>> GetAllAsync(CancellationToken ct)
    {
        var list = await zoneRepo.GetAllAsync(ct);
        return list.Select(ToDto).ToList();
    }

    public async Task<ZoneDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var z = await zoneRepo.GetByIdAsync(id, ct);
        return z == null ? null : ToDto(z);
    }

    public async Task<ZoneDto> CreateAsync(ZoneDto dto, CancellationToken ct)
    {
        var z = new Zone { Id = Guid.NewGuid(), Name = dto.Name, Code = dto.Code, IsActive = dto.IsActive };
        await zoneRepo.AddAsync(z, ct);
        return ToDto(z);
    }

    public async Task<ZoneDto?> UpdateAsync(Guid id, ZoneDto dto, CancellationToken ct)
    {
        var z = await zoneRepo.GetByIdAsync(id, ct);
        if (z == null) return null;
        z.Name = dto.Name;
        z.Code = dto.Code;
        z.IsActive = dto.IsActive;
        await zoneRepo.UpdateAsync(z, ct);
        return ToDto(z);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var z = await zoneRepo.GetByIdAsync(id, ct);
        if (z == null) return false;
        var stationCount = await zoneRepo.CountStationsInZoneAsync(id, ct);
        if (stationCount > 0)
            throw new InvalidOperationException($"Cannot delete zone: it has {stationCount} station(s). Remove or reassign stations first.");
        await zoneRepo.DeleteAsync(z, ct);
        return true;
    }

    private static ZoneDto ToDto(Zone z) => new()
    {
        Id = z.Id,
        Name = z.Name,
        Code = z.Code,
        IsActive = z.IsActive,
        StationCount = z.SortingStations?.Count ?? 0
    };
}
