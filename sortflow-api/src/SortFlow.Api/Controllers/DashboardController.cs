using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SortFlow.Application.Abstractions;
using SortFlow.Application.Models;

namespace SortFlow.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly ISortingEventRepository _sortingEventRepository;
    private readonly ISortingExceptionRepository _sortingExceptionRepository;

    public DashboardController(
        ISortingEventRepository sortingEventRepository,
        ISortingExceptionRepository sortingExceptionRepository)
    {
        _sortingEventRepository = sortingEventRepository;
        _sortingExceptionRepository = sortingExceptionRepository;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(CancellationToken cancellationToken)
    {
        var lastHour = DateTimeOffset.UtcNow.AddHours(-1);
        var lastFiveMinutes = DateTimeOffset.UtcNow.AddMinutes(-5);

        var totalLastHour = await _sortingEventRepository.CountSinceAsync(lastHour, cancellationToken);
        var successfulLastHour = await _sortingEventRepository.CountSuccessfulSinceAsync(lastHour, cancellationToken);
        var exceptionsLastHour = await _sortingExceptionRepository.CountSinceAsync(lastHour, cancellationToken);
        var lastFiveMinutesCount = await _sortingEventRepository.CountSinceAsync(lastFiveMinutes, cancellationToken);

        var summary = new DashboardSummaryDto
        {
            TotalEventsLastHour = totalLastHour,
            SuccessfulEventsLastHour = successfulLastHour,
            ExceptionsLastHour = exceptionsLastHour,
            ItemsPerMinute = Math.Round(lastFiveMinutesCount / 5d, 2),
            ItemsPerHour = totalLastHour,
            GeneratedAtUtc = DateTimeOffset.UtcNow
        };

        return Ok(summary);
    }
}
