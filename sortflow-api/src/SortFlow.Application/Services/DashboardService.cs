using SortFlow.Application.Abstractions;
using SortFlow.Application.Models;
using SortFlow.Domain.Enums;

namespace SortFlow.Application.Services;

public class DashboardService(
    ISortingEventRepository eventRepo,
    ISortingExceptionRepository exceptionRepo,
    IAppSettingsRepository settingsRepo) : IDashboardService
{
    public async Task<DashboardSummaryDto> GetSummaryAsync(int? windowMinutes, DateTimeOffset? timeFrom, DateTimeOffset? timeTo, CancellationToken ct)
    {
        int minutes = windowMinutes ?? 60;
        var settings = await settingsRepo.GetSingleAsync(ct);
        if (settings != null && windowMinutes == null)
            minutes = settings.DashboardWindowMinutes;

        var now = DateTimeOffset.UtcNow;
        DateTimeOffset from = timeFrom ?? now.AddMinutes(-minutes);
        DateTimeOffset to = timeTo ?? now;
        if (from > to) (from, to) = (to, from);

        var todayStart = new DateTimeOffset(now.UtcDateTime.Date, TimeSpan.Zero);

        var totalInWindow = await eventRepo.CountByDateRangeAsync(from, to, ct);
        var successfulInWindow = await eventRepo.CountSuccessfulByDateRangeAsync(from, to, ct);
        var exceptionsInWindow = await exceptionRepo.CountByDateRangeAsync(from, to, ct);

        var totalToday = await eventRepo.CountByDateRangeAsync(todayStart, now.AddMinutes(1), ct);
        var successfulToday = await eventRepo.CountSuccessfulByDateRangeAsync(todayStart, now.AddMinutes(1), ct);

        var windowMinutesD = (to - from).TotalMinutes;
        if (windowMinutesD < 0.5) windowMinutesD = 1;

        var byType = await exceptionRepo.CountByTypeAndDateRangeAsync(from, to, ct);
        var byCategory = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["OK"] = successfulInWindow,
            ["InvalidPostalCode"] = byType.GetValueOrDefault(ExceptionType.InvalidPostalCode, 0),
            ["DamagedLabel"] = byType.GetValueOrDefault(ExceptionType.DamagedLabel, 0),
            ["AddressMismatch"] = byType.GetValueOrDefault(ExceptionType.AddressMismatch, 0)
        };
        byCategory["Exceptions"] = exceptionsInWindow;

        return new DashboardSummaryDto
        {
            ItemsPerMinute = totalInWindow > 0 ? Math.Round(totalInWindow / windowMinutesD, 2) : 0,
            ItemsPerHour = (int)Math.Round(totalInWindow * 60.0 / windowMinutesD),
            TotalToday = totalToday,
            SuccessRate = totalToday > 0 ? Math.Round(100.0 * successfulToday / totalToday, 2) : 0,
            TotalEventsLastHour = totalInWindow,
            SuccessfulEventsLastHour = successfulInWindow,
            ExceptionsLastHour = exceptionsInWindow,
            EventsByCategory = byCategory,
            GeneratedAtUtc = now
        };
    }
}
