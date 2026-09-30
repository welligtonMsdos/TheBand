using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TheBand.CoreApi.Controllers;

public abstract class BaseController : ControllerBase
{
    protected string UserId => User.FindFirstValue("id") ?? string.Empty;
}
