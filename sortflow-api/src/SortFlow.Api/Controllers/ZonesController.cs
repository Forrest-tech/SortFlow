using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SortFlow.Application.Abstractions;
using SortFlow.Application.Models;

namespace SortFlow.Api.Controllers;

[ApiController]
[Route("api/zones")]
[Authorize]
public class ZonesController : ControllerBase
{
    private readonly IZoneService _zoneService;

    public ZonesController(IZoneService zoneService)
    {
        _zoneService = zoneService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ZoneDto>>> GetAll(CancellationToken ct)
    {
        var list = await _zoneService.GetAllAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ZoneDto>> GetById(Guid id, CancellationToken ct)
    {
        var z = await _zoneService.GetByIdAsync(id, ct);
        if (z == null) return NotFound();
        return Ok(z);
    }

    [HttpPost]
    public async Task<ActionResult<ZoneDto>> Create([FromBody] ZoneDto dto, CancellationToken ct)
    {
        var created = await _zoneService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ZoneDto>> Update(Guid id, [FromBody] ZoneDto dto, CancellationToken ct)
    {
        var updated = await _zoneService.UpdateAsync(id, dto, ct);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            var ok = await _zoneService.DeleteAsync(id, ct);
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
