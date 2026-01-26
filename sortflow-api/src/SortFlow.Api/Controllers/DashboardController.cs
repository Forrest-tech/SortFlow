using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SortFlow.Application.Abstractions;

namespace SortFlow.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] int? windowMinutes,
        [FromQuery] DateTimeOffset? timeFrom,
        [FromQuery] DateTimeOffset? timeTo,
        CancellationToken cancellationToken)
    {
        var summary = await _dashboardService.GetSummaryAsync(windowMinutes, timeFrom, timeTo, cancellationToken);
        return Ok(summary);
    }
}
