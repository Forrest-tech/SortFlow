using Microsoft.EntityFrameworkCore;
using SortFlow.Domain.Entities;

namespace SortFlow.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(SortFlowDbContext db, CancellationToken ct = default)
    {
        await SeedZonesAndStationsAsync(db, ct);
        await SeedAppSettingsAsync(db, ct);
    }

    private static async Task SeedZonesAndStationsAsync(SortFlowDbContext db, CancellationToken ct)
    {
        if (await db.Zones.AnyAsync(ct))
            return;

        var now = DateTimeOffset.UtcNow;
        var zones = new[]
        {
            new Zone { Id = Guid.NewGuid(), Name = "North", Code = "N", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new Zone { Id = Guid.NewGuid(), Name = "South", Code = "S", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new Zone { Id = Guid.NewGuid(), Name = "East", Code = "E", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new Zone { Id = Guid.NewGuid(), Name = "West", Code = "W", IsActive = true, CreatedAt = now, UpdatedAt = now }
        };
        db.Zones.AddRange(zones);

        var stations = new List<SortingStation>();
        foreach (var z in zones)
        {
            stations.Add(new SortingStation
            {
                Id = Guid.NewGuid(),
                Name = $"{z.Name}-1",
                StationCode = $"{z.Code}1",
                ZoneId = z.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            });
        }
        db.SortingStations.AddRange(stations);
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedAppSettingsAsync(SortFlowDbContext db, CancellationToken ct)
    {
        if (await db.AppSettings.AnyAsync(ct))
            return;

        var now = DateTimeOffset.UtcNow;
        db.AppSettings.Add(new AppSettings
        {
            Id = Guid.NewGuid(),
            GeneratorRatePerSecond = 1.0,
            AddressMismatchProbability = 0.03,
            InvalidPostalProbability = 0.02,
            DamagedLabelProbability = 0.03,
            DashboardWindowMinutes = 60,
            EnableModules = "{\"Dashboard\":true,\"Events\":true,\"Exceptions\":true,\"Zones\":true,\"Stations\":true,\"History\":true,\"Settings\":true}",
            CreatedAt = now,
            UpdatedAt = now
        });
        await db.SaveChangesAsync(ct);
    }
}
