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

    public async Task AddRangeAsync(IEnumerable<SortingStation> stations, CancellationToken cancellationToken)
    {
        _dbContext.SortingStations.AddRange(stations);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
