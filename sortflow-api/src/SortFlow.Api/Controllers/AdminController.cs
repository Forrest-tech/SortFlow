using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SortFlow.Application.Abstractions;
using SortFlow.Api.Services;

namespace SortFlow.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IGeneratorState _generatorState;
    private readonly IAppSettingsRepository _settingsRepo;

    public AdminController(IGeneratorState generatorState, IAppSettingsRepository settingsRepo)
    {
        _generatorState = generatorState;
        _settingsRepo = settingsRepo;
    }

    [HttpPost("generator/start")]
    public IActionResult GeneratorStart()
    {
        _generatorState.IsRunning = true;
        return Ok(new { status = "started" });
    }

    [HttpPost("generator/stop")]
    public IActionResult GeneratorStop()
    {
        _generatorState.IsRunning = false;
        return Ok(new { status = "stopped" });
    }

    [HttpGet("generator/status")]
    public async Task<IActionResult> GeneratorStatus(CancellationToken ct)
    {
        var settings = await _settingsRepo.GetSingleAsync(ct);
        var rate = settings?.GeneratorRatePerSecond ?? 1.0;
        return Ok(new { isRunning = _generatorState.IsRunning, ratePerSecond = rate });
    }
}
