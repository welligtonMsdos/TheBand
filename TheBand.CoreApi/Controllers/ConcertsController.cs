using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;

namespace TheBand.CoreApi.Controllers;

[ApiController]
[Authorize]
[Route("api/concerts")]
public sealed class ConcertsController : BaseController
{
    private readonly IConcertService _service;

    public ConcertsController(IConcertService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ConcertDto>> Create(
        [FromBody] CreateConcertDto request,
        CancellationToken cancellationToken)
    {
        var concert = await _service.CreateAsync(UserId, request, cancellationToken);

        return CreatedAtAction(nameof(GetByGuid), new { guid = concert.Guid }, concert);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ConcertDto>>> GetAll(
        CancellationToken cancellationToken) =>
        Ok(await _service.GetAllAsync(UserId, cancellationToken));

    [HttpGet("price-by-year")]
    public async Task<ActionResult<IReadOnlyCollection<ConcertPriceByYearDto>>> GetPriceByYear(
        CancellationToken cancellationToken) =>
        Ok(await _service.GetPriceByYearAsync(UserId, cancellationToken));

    [HttpGet("upcoming")]
    public async Task<ActionResult<IReadOnlyCollection<ConcertDto>>> GetUpcoming(
        CancellationToken cancellationToken) =>
        Ok(await _service.GetUpcomingAsync(UserId, cancellationToken));

    [HttpGet("past")]
    public async Task<ActionResult<IReadOnlyCollection<ConcertDto>>> GetPast(
        CancellationToken cancellationToken) =>
        Ok(await _service.GetPastAsync(UserId, cancellationToken));

    [HttpGet("{guid:guid}")]
    public async Task<ActionResult<ConcertDto>> GetByGuid(
        Guid guid,
        CancellationToken cancellationToken)
    {
        var concert = await _service.GetByGuidAsync(guid, UserId, cancellationToken);

        return concert is null ? NotFound() : Ok(concert);
    }

    [HttpPut("{guid:guid}")]
    public async Task<ActionResult<ConcertDto>> Update(
        Guid guid,
        [FromBody] UpdateConcertDto request,
        CancellationToken cancellationToken)
    {
        var concert = await _service.UpdateAsync(guid, UserId, request, cancellationToken);

        return concert is null ? NotFound() : Ok(concert);
    }

    [HttpDelete("{guid:guid}")]
    public async Task<ActionResult> Delete(
        Guid guid,
        CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(guid, UserId, cancellationToken);

        return deleted ? Ok(new { message = "que foi deletado com sucesso" }) : NotFound();
    }
}
