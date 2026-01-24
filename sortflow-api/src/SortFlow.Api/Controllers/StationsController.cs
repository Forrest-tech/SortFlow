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
    private readonly ISortingStationRepository _stationRepository;

    public StationsController(ISortingStationRepository stationRepository)
    {
        _stationRepository = stationRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StationDto>>> GetAll(CancellationToken cancellationToken)
    {
        var stations = await _stationRepository.GetAllAsync(cancellationToken);
        var dtos = stations.Select(s => new StationDto
        {
            Id = s.Id,
            Name = s.Name,
            StationCode = s.StationCode,
            IsActive = s.IsActive,
            ZoneId = s.ZoneId,
            ZoneName = s.Zone?.Name ?? string.Empty
        }).ToList();
        return Ok(dtos);
    }
}
