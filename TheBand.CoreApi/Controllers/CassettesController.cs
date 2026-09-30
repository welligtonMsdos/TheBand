using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;

namespace TheBand.CoreApi.Controllers;

[ApiController]
[Authorize]
[Route("api/cassettes")]
public sealed class CassettesController : BaseController
{
    private readonly ICassetteService _service;

    public CassettesController(ICassetteService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<CassetteDto>> Create(
        [FromBody] CreateCassetteDto request,
        CancellationToken cancellationToken)
    {
        var cassette = await _service.CreateAsync(UserId, request, cancellationToken);

        return CreatedAtAction(nameof(GetByGuid), new { guid = cassette.Guid }, cassette);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CassetteDto>>> GetAll(
        CancellationToken cancellationToken) =>
        Ok(await _service.GetAllAsync(UserId, cancellationToken));

    [HttpGet("{guid:guid}")]
    public async Task<ActionResult<CassetteDto>> GetByGuid(
        Guid guid,
        CancellationToken cancellationToken)
    {
        var cassette = await _service.GetByGuidAsync(guid, UserId, cancellationToken);

        return cassette is null ? NotFound() : Ok(cassette);
    }

    [HttpPut("{guid:guid}")]
    public async Task<ActionResult<CassetteDto>> Update(
        Guid guid,
        [FromBody] UpdateCassetteDto request,
        CancellationToken cancellationToken)
    {
        var cassette = await _service.UpdateAsync(guid, UserId, request, cancellationToken);

        return cassette is null ? NotFound() : Ok(cassette);
    }

    [HttpDelete("{guid:guid}")]
    public async Task<IActionResult> Delete(
        Guid guid,
        CancellationToken cancellationToken) =>
        await _service.DeleteAsync(guid, UserId, cancellationToken) ? NoContent() : NotFound();
}
