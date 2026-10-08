using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBand.AuthApi.Middleware;
using TheBand.AuthApplication.Dtos;
using TheBand.AuthApplication.Interfaces;

namespace TheBand.AuthApi.Presentation.Controllers;

[ApiController]

[Route("api/users/password")]

[Authorize]
public sealed class UserPasswordController : BaseController
{
    private readonly IUserService _userService;

    public UserPasswordController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] ChangePasswordDto changePasswordDto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(UserId))
            return Unauthorized(Result<object>.Failure("Usuário não autenticado."));

        var updated = await _userService.ChangePasswordAsync(UserId, changePasswordDto, cancellationToken);

        return updated
            ? Ok(Result<object>.Ok(null!, "Senha salva com sucesso"))
            : NotFound(Result<object>.Failure("Usuário não encontrado."));
    }
}
