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
    private readonly IZoneRepository _zoneRepository;

    public ZonesController(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ZoneDto>>> GetAll(CancellationToken cancellationToken)
    {
        var zones = await _zoneRepository.GetAllAsync(cancellationToken);
        var dtos = zones.Select(z => new ZoneDto
        {
            Id = z.Id,
            Name = z.Name,
            Code = z.Code,
            IsActive = z.IsActive,
            StationCount = z.SortingStations?.Count ?? 0
        }).ToList();
        return Ok(dtos);
    }
}
