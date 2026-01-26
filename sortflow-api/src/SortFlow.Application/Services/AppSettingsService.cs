using SortFlow.Application.Abstractions;
using SortFlow.Application.Models;
using SortFlow.Domain.Entities;

namespace SortFlow.Application.Services;

public class AppSettingsService(IAppSettingsRepository repo) : IAppSettingsService
{
    public async Task<AppSettingsDto?> GetAsync(CancellationToken ct)
    {
        var e = await repo.GetSingleAsync(ct);
        return e == null ? null : ToDto(e);
    }

    public async Task<AppSettingsDto> UpdateAsync(AppSettingsDto dto, CancellationToken ct)
    {
        var e = await repo.GetSingleAsync(ct) ?? throw new InvalidOperationException("AppSettings not found. Run database seed.");
        e.GeneratorRatePerSecond = dto.GeneratorRatePerSecond;
        e.AddressMismatchProbability = dto.AddressMismatchProbability;
        e.InvalidPostalProbability = dto.InvalidPostalProbability;
        e.DamagedLabelProbability = dto.DamagedLabelProbability;
        e.DashboardWindowMinutes = dto.DashboardWindowMinutes;
        e.EnableModules = dto.EnableModules;
        await repo.UpdateAsync(e, ct);
        return ToDto(e);
    }

    private static AppSettingsDto ToDto(AppSettings e) => new()
    {
        Id = e.Id,
        GeneratorRatePerSecond = e.GeneratorRatePerSecond,
        AddressMismatchProbability = e.AddressMismatchProbability,
        InvalidPostalProbability = e.InvalidPostalProbability,
        DamagedLabelProbability = e.DamagedLabelProbability,
        DashboardWindowMinutes = e.DashboardWindowMinutes,
        EnableModules = e.EnableModules,
        UpdatedAt = e.UpdatedAt
    };
}
