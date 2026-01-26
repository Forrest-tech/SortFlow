using Microsoft.Extensions.DependencyInjection;
using SortFlow.Application.Abstractions;
using SortFlow.Application.Services;

namespace SortFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IHistoryService, HistoryService>();
        services.AddScoped<IAppSettingsService, AppSettingsService>();
        services.AddScoped<IZoneService, ZoneService>();
        services.AddScoped<IStationService, StationService>();
        return services;
    }
}
