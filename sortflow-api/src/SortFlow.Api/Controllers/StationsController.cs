using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SortFlow.Application.Abstractions;
using SortFlow.Application.Models;

namespace SortFlow.Api.Controllers;

[ApiController]
[Route("api/stations")]
[Authorize]
public class StationsController : ControllerBase
{
    private readonly IStationService _stationService;

    public StationsController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StationDto>>> GetAll(CancellationToken ct)
    {
        var list = await _stationService.GetAllAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StationDto>> GetById(Guid id, CancellationToken ct)
    {
        var s = await _stationService.GetByIdAsync(id, ct);
        if (s == null) return NotFound();
        return Ok(s);
    }

    [HttpPost]
    public async Task<ActionResult<StationDto>> Create([FromBody] StationDto dto, CancellationToken ct)
    {
        try
        {
            var created = await _stationService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StationDto>> Update(Guid id, [FromBody] StationDto dto, CancellationToken ct)
    {
        try
        {
            var updated = await _stationService.UpdateAsync(id, dto, ct);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _stationService.DeleteAsync(id, ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}
