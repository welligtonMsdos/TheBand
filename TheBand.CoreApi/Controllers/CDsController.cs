using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBand.CoreApi.Middleware;
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
    public async Task<IActionResult> Post(
        [FromBody] CreateCDDto createCDDto, 
        CancellationToken cancellationToken)
    {
        var cd = await _service.CreateAsync(
            UserId, 
            createCDDto, 
            cancellationToken);

        return CreatedAtAction(
            nameof(GetByGuid), 
            new { guid = cd.Guid }, 
            Result<CDDto>.Ok(cd));
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var cds = await _service.GetAllAsync(
            UserId, 
            cancellationToken);

        return cds is null
                    ? NotFound(Result<object>.Failure("CDs não encontrados."))
                    : Ok(Result<IReadOnlyCollection<CDDto>>.Ok(cds));
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> GetByGuid(
        Guid guid, 
        CancellationToken cancellationToken)
    {
        var cd = await _service.GetByGuidAsync(
            guid, 
            UserId, 
            cancellationToken);

        return cd is null
            ? NotFound(Result<object>.Failure("CD não encontrado."))
            : Ok(Result<CDDto>.Ok(cd));
    }

    [HttpPut("{guid:guid}")]
    public async Task<ActionResult<CDDto>> Update(
       Guid guid,
       [FromBody] UpdateCDDto updateCDDto,
       CancellationToken cancellationToken)
    {
        var cd = await _service.UpdateAsync(
            guid, UserId,
            updateCDDto, 
            cancellationToken);

        return cd is null ? NotFound() : Ok(cd);
    }

    [HttpDelete("{guid:guid}")]
    public async Task<IActionResult> Delete(Guid guid)
    {
        var deleted = await _service.DeleteAsync(
            guid, 
            UserId, 
            CancellationToken.None);

        return deleted
            ? NoContent()
            : NotFound(Result<object>.Failure("CD não encontrado."));
    }
}
