using Microsoft.AspNetCore.Mvc;
using TheBand.AuthApplication.Dtos;
using TheBand.AuthApplication.Interfaces;

namespace TheBand.AuthApi.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("[Action]")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        var user = await _userService.GetDataLoginAsync(userLoginDto);             

        return Ok(user);
    }
}
