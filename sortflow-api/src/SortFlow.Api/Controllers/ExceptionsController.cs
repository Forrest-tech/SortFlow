using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SortFlow.Application.Abstractions;
using SortFlow.Application.Models;

namespace SortFlow.Api.Controllers;

[ApiController]
[Route("api/exceptions")]
[Authorize]
public class ExceptionsController : ControllerBase
{
    private readonly ISortingExceptionRepository _exceptionRepository;

    public ExceptionsController(ISortingExceptionRepository exceptionRepository)
    {
        _exceptionRepository = exceptionRepository;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<ExceptionDto>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDir = null,
        [FromQuery] Guid? zoneId = null,
        [FromQuery] Guid? stationId = null,
        [FromQuery] DateTimeOffset? timeFrom = null,
        [FromQuery] DateTimeOffset? timeTo = null,
        [FromQuery] string? exceptionType = null,
        CancellationToken cancellationToken = default)
    {
        var filter = new ExceptionsFilterDto
        {
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDir = sortDir,
            ZoneId = zoneId,
            StationId = stationId,
            TimeFrom = timeFrom,
            TimeTo = timeTo,
            ExceptionType = exceptionType
        };
        var paged = await _exceptionRepository.GetPagedAsync(filter, cancellationToken);
        var dtos = paged.Items.Select(ex => new ExceptionDto
        {
            Id = ex.Id,
            ExceptionType = ex.ExceptionType.ToString(),
            Details = ex.Details,
            ItemId = ex.SortingEvent?.ItemId ?? string.Empty,
            StationName = ex.SortingEvent?.SortingStation?.Name ?? string.Empty,
            CreatedAtUtc = ex.Timestamp
        }).ToList();

        return Ok(new PagedResultDto<ExceptionDto> { Items = dtos, TotalCount = paged.TotalCount, Page = paged.Page, PageSize = paged.PageSize });
    }
}
