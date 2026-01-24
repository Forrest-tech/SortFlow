using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SortFlow.Application.Abstractions;
using SortFlow.Application.Models;

namespace SortFlow.Api.Controllers;

[ApiController]
[Route("api/events")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly ISortingEventRepository _eventRepository;

    public EventsController(ISortingEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetRecent(
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 1, 200);
        var events = await _eventRepository.GetRecentAsync(limit, cancellationToken);

        var dtos = events.Select(e => new EventDto
        {
            Id = e.Id,
            ItemId = e.ItemId,
            PostalCode = e.PostalCode,
            ProcessedAtUtc = e.ProcessedAt,
            IsSuccessful = e.IsSuccessful,
            ExceptionType = e.SortingException != null ? e.SortingException.ExceptionType.ToString() : null,
            StationName = e.SortingStation?.Name ?? string.Empty,
            ZoneName = e.SortingStation?.Zone?.Name ?? string.Empty
        }).ToList();

        return Ok(dtos);
    }
}
