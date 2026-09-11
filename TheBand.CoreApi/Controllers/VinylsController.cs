using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBand.CoreApi.Filters;
using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;

namespace TheBand.CoreApi.Controllers;

[ApiController]
[Authorize]
[ServiceFilter<UserIdMatchesTokenFilter>]
[Route("api/[controller]")]
public sealed class VinylsController : ControllerBase
{
    private readonly IVinylService _service;

    public VinylsController(IVinylService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<VinylDto>> Create(
        [FromQuery] string userId,
        [FromBody] CreateVinylDto request,
        CancellationToken cancellationToken)
    {
        var vinyl = await _service.CreateAsync(userId, request, cancellationToken);

        return CreatedAtAction(nameof(GetByGuid), new { guid = vinyl.Guid, userId }, vinyl);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<VinylDto>>> GetAll(
        [FromQuery] string userId,
        CancellationToken cancellationToken) =>
        Ok(await _service.GetAllAsync(userId, cancellationToken));

    [HttpGet("{guid:guid}")]
    public async Task<ActionResult<VinylDto>> GetByGuid(
        Guid guid,
        [FromQuery] string userId,
        CancellationToken cancellationToken)
    {
        var vinyl = await _service.GetByGuidAsync(guid, userId, cancellationToken);

        return vinyl is null ? NotFound() : Ok(vinyl);
    }

    [HttpPut("{guid:guid}")]
    public async Task<ActionResult<VinylDto>> Update(
        Guid guid,
        [FromQuery] string userId,
        [FromBody] UpdateVinylDto request,
        CancellationToken cancellationToken)
    {
        var vinyl = await _service.UpdateAsync(guid, userId, request, cancellationToken);

        return vinyl is null ? NotFound() : Ok(vinyl);
    }

    [HttpDelete("{guid:guid}")]
    public async Task<IActionResult> Delete(
        Guid guid,
        [FromQuery] string userId,
        CancellationToken cancellationToken) =>
        await _service.DeleteAsync(guid, userId, cancellationToken) ? NoContent() : NotFound();
}
