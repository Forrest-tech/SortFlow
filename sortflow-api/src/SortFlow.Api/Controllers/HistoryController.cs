using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SortFlow.Application.Abstractions;

namespace SortFlow.Api.Controllers;

[ApiController]
[Route("api/history")]
[Authorize]
public class HistoryController : ControllerBase
{
    private readonly IHistoryService _historyService;

    public HistoryController(IHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAggregated(
        [FromQuery] string groupBy = "day",
        [FromQuery] DateTimeOffset? from = null,
        [FromQuery] DateTimeOffset? to = null,
        CancellationToken ct = default)
    {
        var toUtc = to ?? DateTimeOffset.UtcNow;
        var fromUtc = from ?? toUtc.AddDays(-30);
        var list = await _historyService.GetAggregatedAsync(groupBy, fromUtc, toUtc, ct);
        return Ok(list);
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportCsv(
        [FromQuery] DateTimeOffset? from = null,
        [FromQuery] DateTimeOffset? to = null,
        CancellationToken ct = default)
    {
        var toUtc = to ?? DateTimeOffset.UtcNow;
        var fromUtc = from ?? toUtc.AddDays(-7);
        var bytes = await _historyService.GetExportCsvAsync(fromUtc, toUtc, ct);
        return File(bytes, "text/csv", $"sortflow-events-{fromUtc:yyyyMMdd}-{toUtc:yyyyMMdd}.csv");
    }
}
