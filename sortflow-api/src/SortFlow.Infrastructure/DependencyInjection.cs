using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SortFlow.Application.Abstractions;
using SortFlow.Infrastructure.Data;
using SortFlow.Infrastructure.Repositories;

namespace SortFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SortFlowDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("SortFlowDb"));
        });

        services.AddScoped<IAppSettingsRepository, AppSettingsRepository>();
        services.AddScoped<IHistoryRepository, HistoryRepository>();
        services.AddScoped<ISortingEventRepository, SortingEventRepository>();
        services.AddScoped<ISortingExceptionRepository, SortingExceptionRepository>();
        services.AddScoped<ISortingStationRepository, SortingStationRepository>();
        services.AddScoped<IZoneRepository, ZoneRepository>();

        return services;
    }
}
