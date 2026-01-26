using Microsoft.EntityFrameworkCore;
using SortFlow.Application.Abstractions;
using SortFlow.Domain.Entities;
using SortFlow.Infrastructure.Data;

namespace SortFlow.Infrastructure.Repositories;

public class SortingStationRepository : ISortingStationRepository
{
    private readonly SortFlowDbContext _dbContext;

    public SortingStationRepository(SortFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<SortingStation>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SortingStations.Include(s => s.Zone).ToListAsync(cancellationToken);
    }

    public async Task<SortingStation?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.SortingStations.Include(s => s.Zone).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<SortingStation>> GetByZoneIdAsync(Guid zoneId, CancellationToken cancellationToken)
    {
        return await _dbContext.SortingStations.Include(s => s.Zone).Where(s => s.ZoneId == zoneId).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SortingStation station, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        station.CreatedAt = now;
        station.UpdatedAt = now;
        _dbContext.SortingStations.Add(station);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SortingStation station, CancellationToken cancellationToken)
    {
        station.UpdatedAt = DateTimeOffset.UtcNow;
        _dbContext.SortingStations.Update(station);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(SortingStation station, CancellationToken cancellationToken)
    {
        _dbContext.SortingStations.Remove(station);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<SortingStation> stations, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var s in stations)
        {
            s.CreatedAt = now;
            s.UpdatedAt = now;
        }
        _dbContext.SortingStations.AddRange(stations);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
