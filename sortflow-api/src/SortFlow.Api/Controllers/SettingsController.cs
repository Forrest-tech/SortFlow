using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SortFlow.Application.Abstractions;
using SortFlow.Application.Models;

namespace SortFlow.Api.Controllers;

[ApiController]
[Route("api/settings")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly IAppSettingsService _settingsService;

    public SettingsController(IAppSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var dto = await _settingsService.GetAsync(ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] AppSettingsDto dto, CancellationToken ct)
    {
        var updated = await _settingsService.UpdateAsync(dto, ct);
        return Ok(updated);
    }
}
