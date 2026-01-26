using SortFlow.Application.Models;

namespace SortFlow.Application.Abstractions;

public interface IAppSettingsService
{
    Task<AppSettingsDto?> GetAsync(CancellationToken ct);
    Task<AppSettingsDto> UpdateAsync(AppSettingsDto dto, CancellationToken ct);
}
