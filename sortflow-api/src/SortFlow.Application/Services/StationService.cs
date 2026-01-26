using SortFlow.Application.Abstractions;
using SortFlow.Application.Models;
using SortFlow.Domain.Entities;

namespace SortFlow.Application.Services;

public class StationService(ISortingStationRepository stationRepo, IZoneRepository zoneRepo) : IStationService
{
    public async Task<IReadOnlyList<StationDto>> GetAllAsync(CancellationToken ct)
    {
        var list = await stationRepo.GetAllAsync(ct);
        return list.Select(ToDto).ToList();
    }

    public async Task<StationDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var s = await stationRepo.GetByIdAsync(id, ct);
        return s == null ? null : ToDto(s);
    }

    public async Task<StationDto> CreateAsync(StationDto dto, CancellationToken ct)
    {
        _ = await zoneRepo.GetByIdAsync(dto.ZoneId, ct) ?? throw new InvalidOperationException("Zone not found.");
        var s = new SortingStation { Id = Guid.NewGuid(), Name = dto.Name, StationCode = dto.StationCode ?? dto.Name, IsActive = dto.IsActive, ZoneId = dto.ZoneId };
        await stationRepo.AddAsync(s, ct);
        return (await GetByIdAsync(s.Id, ct))!;
    }

    public async Task<StationDto?> UpdateAsync(Guid id, StationDto dto, CancellationToken ct)
    {
        var s = await stationRepo.GetByIdAsync(id, ct);
        if (s == null) return null;
        _ = await zoneRepo.GetByIdAsync(dto.ZoneId, ct) ?? throw new InvalidOperationException("Zone not found.");
        s.Name = dto.Name;
        s.StationCode = dto.StationCode ?? s.StationCode;
        s.IsActive = dto.IsActive;
        s.ZoneId = dto.ZoneId;
        await stationRepo.UpdateAsync(s, ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var s = await stationRepo.GetByIdAsync(id, ct);
        if (s == null) return false;
        await stationRepo.DeleteAsync(s, ct);
        return true;
    }

    private static StationDto ToDto(SortingStation s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        StationCode = s.StationCode,
        IsActive = s.IsActive,
        ZoneId = s.ZoneId,
        ZoneName = s.Zone?.Name ?? string.Empty
    };
}
