using Microsoft.EntityFrameworkCore;
using SortFlow.Application.Abstractions;
using SortFlow.Domain.Entities;
using SortFlow.Infrastructure.Data;

namespace SortFlow.Infrastructure.Repositories;

public class ZoneRepository : IZoneRepository
{
    private readonly SortFlowDbContext _dbContext;

    public ZoneRepository(SortFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Zone>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Zones.Include(z => z.SortingStations).ToListAsync(cancellationToken);
    }

    public async Task<Zone?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Zones.Include(z => z.SortingStations).FirstOrDefaultAsync(z => z.Id == id, cancellationToken);
    }

    public async Task AddAsync(Zone zone, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        zone.CreatedAt = now;
        zone.UpdatedAt = now;
        _dbContext.Zones.Add(zone);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Zone zone, CancellationToken cancellationToken)
    {
        zone.UpdatedAt = DateTimeOffset.UtcNow;
        _dbContext.Zones.Update(zone);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Zone zone, CancellationToken cancellationToken)
    {
        _dbContext.Zones.Remove(zone);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<int> CountStationsInZoneAsync(Guid zoneId, CancellationToken cancellationToken)
    {
        return _dbContext.SortingStations.CountAsync(s => s.ZoneId == zoneId, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Zone> zones, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var z in zones)
        {
            z.CreatedAt = now;
            z.UpdatedAt = now;
        }
        _dbContext.Zones.AddRange(zones);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
