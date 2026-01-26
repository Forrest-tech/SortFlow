using Microsoft.EntityFrameworkCore;
using SortFlow.Application.Abstractions;
using SortFlow.Domain.Entities;
using SortFlow.Infrastructure.Data;

namespace SortFlow.Infrastructure.Repositories;

public class AppSettingsRepository : IAppSettingsRepository
{
    private readonly SortFlowDbContext _dbContext;

    public AppSettingsRepository(SortFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppSettings?> GetSingleAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.AppSettings.OrderBy(x => x.CreatedAt).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(AppSettings entity, CancellationToken cancellationToken)
    {
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        _dbContext.AppSettings.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
