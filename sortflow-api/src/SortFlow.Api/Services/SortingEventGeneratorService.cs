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
    private readonly IGeneratorState _generatorState;
    private readonly ILogger<SortingEventGeneratorService> _logger;
    private readonly Random _random = new();

    public SortingEventGeneratorService(
        IServiceProvider serviceProvider,
        IHubContext<DashboardHub> hubContext,
        IGeneratorState generatorState,
        ILogger<SortingEventGeneratorService> logger)
    {
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
        _generatorState = generatorState;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Sorting event generator started. IsRunning={IsRunning}", _generatorState.IsRunning);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_generatorState.IsRunning)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                continue;
            }

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var stationRepository = scope.ServiceProvider.GetRequiredService<ISortingStationRepository>();
                var eventRepository = scope.ServiceProvider.GetRequiredService<ISortingEventRepository>();
                var exceptionRepository = scope.ServiceProvider.GetRequiredService<ISortingExceptionRepository>();
                var settingsRepo = scope.ServiceProvider.GetRequiredService<IAppSettingsRepository>();

                var stations = await stationRepository.GetAllAsync(stoppingToken);
                if (stations.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    continue;
                }

                var settings = await settingsRepo.GetSingleAsync(stoppingToken);
                var rate = (settings?.GeneratorRatePerSecond ?? 1.0);
                if (rate < 0.1) rate = 1.0;

                var station = stations[_random.Next(stations.Count)];
                var postalCode = $"{PostalPrefixes[_random.Next(PostalPrefixes.Length)]}{_random.Next(1000, 9999)}";
                var itemId = $"ITEM-{_random.Next(100000, 999999)}";

                var pAddr = (double)(settings?.AddressMismatchProbability ?? 0.03);
                var pPostal = (double)(settings?.InvalidPostalProbability ?? 0.02);
                var pDamaged = (double)(settings?.DamagedLabelProbability ?? 0.03);
                var pEx = pAddr + pPostal + pDamaged;
                var r = _random.NextDouble();
                var isSuccessful = r >= pEx;
                ExceptionType? exType = null;
                if (!isSuccessful)
                {
                    if (r < pPostal) exType = ExceptionType.InvalidPostalCode;
                    else if (r < pPostal + pDamaged) exType = ExceptionType.DamagedLabel;
                    else exType = ExceptionType.AddressMismatch;
                }

                var now = DateTimeOffset.UtcNow;
                var sortingEvent = new SortingEvent
                {
                    Id = Guid.NewGuid(),
                    ItemId = itemId,
                    PostalCode = postalCode,
                    StationId = station.Id,
                    ZoneId = station.ZoneId,
                    Result = isSuccessful ? "OK" : exType!.Value.ToString(),
                    Timestamp = now,
                    CreatedAt = now
                };

                await eventRepository.AddAsync(sortingEvent, stoppingToken);

                SortingException? sortingException = null;
                if (exType.HasValue)
                {
                    sortingException = new SortingException
                    {
                        Id = Guid.NewGuid(),
                        SortingEventId = sortingEvent.Id,
                        ExceptionType = exType.Value,
                        Details = "Auto-detected exception from scan pipeline.",
                        Timestamp = now,
                        CreatedAt = now
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
                        ProcessedAt = sortingEvent.Timestamp,
                        IsSuccessful = sortingEvent.IsSuccessful,
                        StationName = station.Name,
                        ZoneName = station.Zone?.Name ?? string.Empty,
                        ExceptionType = sortingException?.ExceptionType.ToString()
                    },
                    stoppingToken);

                var dashboardSvc = scope.ServiceProvider.GetRequiredService<IDashboardService>();
                var summary = await dashboardSvc.GetSummaryAsync(null, null, null, stoppingToken);
                await _hubContext.Clients.All.SendAsync("dashboard:summaryUpdated", summary, stoppingToken);

                var eventDto = new
                {
                    Id = sortingEvent.Id,
                    ItemId = sortingEvent.ItemId,
                    PostalCode = sortingEvent.PostalCode,
                    ProcessedAtUtc = sortingEvent.Timestamp,
                    IsSuccessful = sortingEvent.IsSuccessful,
                    ExceptionType = sortingException?.ExceptionType.ToString(),
                    StationName = station.Name,
                    ZoneName = station.Zone?.Name ?? string.Empty
                };
                await _hubContext.Clients.All.SendAsync("events:newBatch", new[] { eventDto }, stoppingToken);

                if (sortingException != null)
                {
                    var exDto = new
                    {
                        Id = sortingException.Id,
                        ExceptionType = sortingException.ExceptionType.ToString(),
                        Details = sortingException.Details,
                        ItemId = sortingEvent.ItemId,
                        StationName = station.Name,
                        CreatedAtUtc = sortingException.Timestamp
                    };
                    await _hubContext.Clients.All.SendAsync("exceptions:newBatch", new[] { exDto }, stoppingToken);
                }

                var delaySec = 1.0 / rate;
                if (delaySec > 0.01)
                    await Task.Delay(TimeSpan.FromSeconds(delaySec), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate sorting event.");
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
