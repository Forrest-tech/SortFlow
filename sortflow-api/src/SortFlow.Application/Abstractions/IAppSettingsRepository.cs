using SortFlow.Domain.Entities;

namespace SortFlow.Application.Abstractions;

public interface IAppSettingsRepository
{
    Task<AppSettings?> GetSingleAsync(CancellationToken cancellationToken);
    Task UpdateAsync(AppSettings entity, CancellationToken cancellationToken);
}
