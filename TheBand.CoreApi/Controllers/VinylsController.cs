using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;

namespace TheBand.CoreApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class VinylsController : BaseController
{
    private readonly IVinylService _service;

    public VinylsController(IVinylService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<VinylDto>> Create(
        [FromBody] CreateVinylDto request,
        CancellationToken cancellationToken)
    {
        var vinyl = await _service.CreateAsync(UserId, request, cancellationToken);

        return CreatedAtAction(nameof(GetByGuid), new { guid = vinyl.Guid }, vinyl);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<VinylDto>>> GetAll(
        CancellationToken cancellationToken) =>
        Ok(await _service.GetAllAsync(UserId, cancellationToken));

    [HttpGet("{guid:guid}")]
    public async Task<ActionResult<VinylDto>> GetByGuid(
        Guid guid,
        CancellationToken cancellationToken)
    {
        var vinyl = await _service.GetByGuidAsync(guid, UserId, cancellationToken);

        return vinyl is null ? NotFound() : Ok(vinyl);
    }

    [HttpPut("{guid:guid}")]
    public async Task<ActionResult<VinylDto>> Update(
        Guid guid,
        [FromBody] UpdateVinylDto request,
        CancellationToken cancellationToken)
    {
        var vinyl = await _service.UpdateAsync(guid, UserId, request, cancellationToken);

        return vinyl is null ? NotFound() : Ok(vinyl);
    }

    [HttpDelete("{guid:guid}")]
    public async Task<IActionResult> Delete(
        Guid guid,
        CancellationToken cancellationToken) =>
        await _service.DeleteAsync(guid, UserId, cancellationToken) ? NoContent() : NotFound();
}
