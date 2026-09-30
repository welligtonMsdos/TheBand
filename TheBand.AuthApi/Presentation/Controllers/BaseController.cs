using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TheBand.AuthApi.Presentation.Controllers;

public abstract class BaseController : ControllerBase
{
    protected string UserId => User.FindFirstValue("id") ?? string.Empty;
}
