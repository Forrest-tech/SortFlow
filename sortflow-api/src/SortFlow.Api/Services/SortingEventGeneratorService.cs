using Microsoft.AspNetCore.SignalR;
using SortFlow.Api.Hubs;
using SortFlow.Application.Abstractions;
using SortFlow.Domain.Entities;
using SortFlow.Domain.Enums;

namespace SortFlow.Api.Services;

public class SortingEventGeneratorService : BackgroundService
{
    private static readonly string[] PostalPrefixes = ["10", "20", "30", "40", "50", "60", "70"];
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<DashboardHub> _hubContext;
    private readonly ILogger<SortingEventGeneratorService> _logger;
    private readonly Random _random = new();

    public SortingEventGeneratorService(
        IServiceProvider serviceProvider,
        IHubContext<DashboardHub> hubContext,
        ILogger<SortingEventGeneratorService> logger)
    {
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Sorting event generator started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var stationRepository = scope.ServiceProvider.GetRequiredService<ISortingStationRepository>();
                var zoneRepository = scope.ServiceProvider.GetRequiredService<IZoneRepository>();
                var eventRepository = scope.ServiceProvider.GetRequiredService<ISortingEventRepository>();
                var exceptionRepository = scope.ServiceProvider.GetRequiredService<ISortingExceptionRepository>();

                var zones = await zoneRepository.GetAllAsync(stoppingToken);
                if (zones.Count == 0)
                {
                    await SeedZonesAsync(zoneRepository, stoppingToken);
                    zones = await zoneRepository.GetAllAsync(stoppingToken);
                }

                var stations = await stationRepository.GetAllAsync(stoppingToken);
                if (stations.Count == 0)
                {
                    await SeedStationsAsync(stationRepository, zones, stoppingToken);
                    stations = await stationRepository.GetAllAsync(stoppingToken);
                }

                var station = stations[_random.Next(stations.Count)];
                var zone = zones[_random.Next(zones.Count)];
                var postalCode = $"{PostalPrefixes[_random.Next(PostalPrefixes.Length)]}{_random.Next(1000, 9999)}";
                var itemId = $"ITEM-{_random.Next(100000, 999999)}";
                var isSuccessful = _random.NextDouble() > 0.08;

                var sortingEvent = new SortingEvent
                {
                    Id = Guid.NewGuid(),
                    ItemId = itemId,
                    PostalCode = postalCode,
                    ProcessedAt = DateTimeOffset.UtcNow,
                    IsSuccessful = isSuccessful,
                    SortingStationId = station.Id,
                    ZoneId = zone.Id
                };

                await eventRepository.AddAsync(sortingEvent, stoppingToken);

                SortingException? sortingException = null;
                if (!isSuccessful)
                {
                    sortingException = new SortingException
                    {
                        Id = Guid.NewGuid(),
                        SortingEventId = sortingEvent.Id,
                        ExceptionType = GetRandomExceptionType(),
                        Details = "Auto-detected exception from scan pipeline.",
                        CreatedAt = DateTimeOffset.UtcNow
                    };

                    await exceptionRepository.AddAsync(sortingException, stoppingToken);
                }

                await _hubContext.Clients.All.SendAsync(
                    "sortingEventReceived",
                    new
                    {
                        sortingEvent.Id,
                        sortingEvent.ItemId,
                        sortingEvent.PostalCode,
                        sortingEvent.ProcessedAt,
                        sortingEvent.IsSuccessful,
                        StationName = station.Name,
                        ZoneName = zone.Name,
                        ExceptionType = sortingException?.ExceptionType.ToString()
                    },
                    stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate sorting event.");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }

    private static async Task SeedZonesAsync(IZoneRepository zoneRepository, CancellationToken cancellationToken)
    {
        var zones = new[]
        {
            new Zone { Id = Guid.NewGuid(), Name = "North Hub", Code = "N1" },
            new Zone { Id = Guid.NewGuid(), Name = "South Hub", Code = "S1" },
            new Zone { Id = Guid.NewGuid(), Name = "East Hub", Code = "E1" },
            new Zone { Id = Guid.NewGuid(), Name = "West Hub", Code = "W1" }
        };

        await zoneRepository.AddRangeAsync(zones, cancellationToken);
    }

    private static async Task SeedStationsAsync(
        ISortingStationRepository stationRepository,
        IReadOnlyList<Zone> zones,
        CancellationToken cancellationToken)
    {
        var stations = zones.Select((zone, index) => new SortingStation
        {
            Id = Guid.NewGuid(),
            Name = $"Station {index + 1}",
            StationCode = $"ST-{index + 1:D2}",
            ZoneId = zone.Id
        });

        await stationRepository.AddRangeAsync(stations, cancellationToken);
    }

    private ExceptionType GetRandomExceptionType()
    {
        var values = Enum.GetValues<ExceptionType>();
        return values[_random.Next(values.Length)];
    }
}
