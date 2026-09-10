using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBand.AuthApplication.Interfaces;
using TheBand.AuthDomain.Enum;

namespace TheBand.AuthApi.Presentation.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.User)}")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await _userService.GetUsersAsync();

        return Ok(users);
    }
}
