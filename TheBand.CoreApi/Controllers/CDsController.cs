using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;

namespace TheBand.CoreApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CDsController : BaseController
{
    private readonly ICDService _service;

    public CDsController(ICDService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<CDDto>> Create(
        [FromBody] CreateCDDto createCDDto, 
        CancellationToken cancellationToken)
    {
        var cd = await _service.CreateAsync(
            UserId, 
            createCDDto, 
            cancellationToken);        

        return CreatedAtAction(nameof(GetByGuid), new { guid = cd.Guid }, cd);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CDDto>>> GetAll(
        CancellationToken cancellationToken) =>
        Ok(await _service.GetAllAsync(UserId, cancellationToken));


    [HttpGet("{guid:guid}")]
    public async Task<ActionResult<CDDto>> GetByGuid(
       Guid guid,
       CancellationToken cancellationToken)
    {
        var cd = await _service.GetByGuidAsync(guid, UserId, cancellationToken);

        return cd is null ? NotFound() : Ok(cd);
    }

    [HttpPut("{guid:guid}")]
    public async Task<ActionResult<CDDto>> Update(
       Guid guid,
       [FromBody] UpdateCDDto request,
       CancellationToken cancellationToken)
    {
        var cd = await _service.UpdateAsync(guid, UserId, request, cancellationToken);

        return cd is null ? NotFound() : Ok(cd);
    }

    [HttpDelete("{guid:guid}")]
    public async Task<IActionResult> Delete(
        Guid guid,
        CancellationToken cancellationToken) =>
        await _service.DeleteAsync(guid, UserId, cancellationToken) ? NoContent() : NotFound();
}
