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
    public async Task<ActionResult<IReadOnlyList<ExceptionDto>>> GetRecentExceptions(
        [FromQuery] int limit = 25,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 1, 200);
        var exceptions = await _exceptionRepository.GetRecentAsync(limit, cancellationToken);

        var response = exceptions.Select(exceptionItem => new ExceptionDto
        {
            Id = exceptionItem.Id,
            ExceptionType = exceptionItem.ExceptionType.ToString(),
            Details = exceptionItem.Details,
            ItemId = exceptionItem.SortingEvent?.ItemId ?? string.Empty,
            StationName = exceptionItem.SortingEvent?.SortingStation?.Name ?? string.Empty,
            CreatedAtUtc = exceptionItem.CreatedAt
        }).ToList();

        return Ok(response);
    }
}
