using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBand.AuthApi.Middleware;
using TheBand.AuthApplication.Dtos;
using TheBand.AuthApplication.Interfaces;
using TheBand.AuthDomain.Enum;

namespace TheBand.AuthApi.Presentation.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var users = await _userService.GetUsersAsync(cancellationToken);

        return Ok(Result<IReadOnlyCollection<UserDto>>.Ok(users));
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetById(string userId, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(userId, cancellationToken);

        return user is null
            ? NotFound(Result<object>.Failure("Usuário não encontrado."))
            : Ok(Result<UserDto>.Ok(user));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        var user = await _userService.CreateAsync(createUserDto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { userId = user._id }, Result<UserDto>.Ok(user));
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> Put(string userId, [FromBody] UpdateUserDto updateUserDto, CancellationToken cancellationToken)
    {
        var user = await _userService.UpdateAsync(userId, updateUserDto, cancellationToken);

        return user is null
            ? NotFound(Result<object>.Failure("Usuário não encontrado."))
            : Ok(Result<UserDto>.Ok(user));
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(string userId, CancellationToken cancellationToken)
    {
        var deleted = await _userService.DeleteAsync(userId, cancellationToken);

        return deleted
            ? NoContent()
            : NotFound(Result<object>.Failure("Usuário não encontrado."));
    }
}
