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
    public async Task<ActionResult<PagedResultDto<EventDto>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDir = null,
        [FromQuery] Guid? zoneId = null,
        [FromQuery] Guid? stationId = null,
        [FromQuery] DateTimeOffset? timeFrom = null,
        [FromQuery] DateTimeOffset? timeTo = null,
        [FromQuery] string? exceptionType = null,
        [FromQuery] string? result = null,
        CancellationToken cancellationToken = default)
    {
        var filter = new EventsFilterDto
        {
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDir = sortDir,
            ZoneId = zoneId,
            StationId = stationId,
            TimeFrom = timeFrom,
            TimeTo = timeTo,
            ExceptionType = exceptionType,
            Result = result
        };
        var paged = await _eventRepository.GetPagedAsync(filter, cancellationToken);
        var dtos = paged.Items.Select(e => new EventDto
        {
            Id = e.Id,
            ItemId = e.ItemId,
            PostalCode = e.PostalCode,
            ProcessedAtUtc = e.Timestamp,
            IsSuccessful = e.IsSuccessful,
            ExceptionType = e.SortingException != null ? e.SortingException.ExceptionType.ToString() : null,
            StationName = e.SortingStation?.Name ?? string.Empty,
            ZoneName = e.SortingStation?.Zone?.Name ?? string.Empty
        }).ToList();

        return Ok(new PagedResultDto<EventDto> { Items = dtos, TotalCount = paged.TotalCount, Page = paged.Page, PageSize = paged.PageSize });
    }
}
