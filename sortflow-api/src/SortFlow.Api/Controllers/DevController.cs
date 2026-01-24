using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SortFlow.Infrastructure.Data;

namespace SortFlow.Api.Controllers;

[ApiController]
[Route("api/dev")]
[AllowAnonymous]
public class DevController : ControllerBase
{
    private readonly SortFlowDbContext _db;
    private readonly IWebHostEnvironment _env;

    public DevController(SortFlowDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    /// <summary>
    /// Development-only: row counts for Zones, Stations, Events, Exceptions. Use to verify the background worker and DB.
    /// </summary>
    [HttpGet("seed-status")]
    public async Task<IActionResult> GetSeedStatus(CancellationToken ct)
    {
        if (!_env.IsDevelopment())
            return NotFound();

        var counts = new
        {
            Zones = await _db.Zones.CountAsync(ct),
            Stations = await _db.SortingStations.CountAsync(ct),
            Events = await _db.SortingEvents.CountAsync(ct),
            Exceptions = await _db.SortingExceptions.CountAsync(ct)
        };
        return Ok(counts);
    }
}
