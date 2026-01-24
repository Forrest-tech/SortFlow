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

    public async Task AddRangeAsync(IEnumerable<Zone> zones, CancellationToken cancellationToken)
    {
        _dbContext.Zones.AddRange(zones);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
