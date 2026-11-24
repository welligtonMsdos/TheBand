using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;

namespace TheBand.CoreApi.Controllers;

[ApiController]
[Route("api/collections")]
public sealed class CollectionsController : BaseController
{
    private readonly ICollectionPathService _service;

    public CollectionsController(ICollectionPathService service)
    {
        _service = service;
    }

    [HttpGet("paths")]
    public async Task<ActionResult<IReadOnlyCollection<CollectionPathDto>>> GetPaths(CancellationToken cancellationToken)
    {
        var paths = await _service.GetActivePathsAsync(cancellationToken);

        return Ok(paths);
    }
}
