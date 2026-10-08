using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheBand.AuthApi.Middleware;
using TheBand.AuthApi.Presentation.Controllers;
using TheBand.AuthApplication.Dtos;
using TheBand.AuthApplication.Interfaces;

namespace TheBand.AuthTests;

public sealed class UserPasswordControllerTests
{
    [Fact]
    public async Task Put_UsesAuthenticatedUserAndReturnsSuccessMessage()
    {
        var service = new FakeUserService { ChangeResult = true };

        var controller = CreateController(service, "authenticated-user");

        var request = new ChangePasswordDto("SenhaAtual123", "NovaSenha456", "NovaSenha456");

        using var cancellation = new CancellationTokenSource();

        var result = await controller.Put(request, cancellation.Token);

        var response = Assert.IsType<OkObjectResult>(result);

        var body = Assert.IsType<Result<object>>(response.Value);

        Assert.True(body.Success);

        Assert.Equal("Senha salva com sucesso", body.Message);

        Assert.Equal("authenticated-user", service.ReceivedUserId);

        Assert.Same(request, service.ReceivedRequest);

        Assert.Equal(cancellation.Token, service.ReceivedCancellationToken);
    }

    [Fact]
    public async Task Put_MissingUser_ReturnsNotFound()
    {
        var controller = CreateController(new FakeUserService(), "missing");

        var result = await controller.Put(new ChangePasswordDto("SenhaAtual123", "NovaSenha456", "NovaSenha456"), default);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Put_MissingUserClaim_ReturnsUnauthorizedWithoutCallingService()
    {
        var service = new FakeUserService();

        var controller = CreateController(service, null);

        var result = await controller.Put(new ChangePasswordDto("SenhaAtual123", "NovaSenha456", "NovaSenha456"), default);

        Assert.IsType<UnauthorizedObjectResult>(result);

        Assert.Null(service.ReceivedUserId);
    }

    [Fact]
    public void Endpoint_RequiresAuthenticationWithoutAdminRestriction()
    {
        var authorization = Assert.Single(typeof(UserPasswordController)
            .GetCustomAttributes(typeof(AuthorizeAttribute), true).Cast<AuthorizeAttribute>());

        Assert.Null(authorization.Roles);

        var route = Assert.Single(typeof(UserPasswordController)
            .GetCustomAttributes(typeof(RouteAttribute), true).Cast<RouteAttribute>());

        Assert.Equal("api/users/password", route.Template);
    }

    private static UserPasswordController CreateController(IUserService service, string? userId)
    {
        var claims = userId is null ? Array.Empty<Claim>() : [new Claim("id", userId)];

        return new UserPasswordController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"))
                }
            }
        };
    }

    private sealed class FakeUserService : IUserService
    {
        public bool ChangeResult { get; init; }

        public string? ReceivedUserId { get; private set; }

        public ChangePasswordDto? ReceivedRequest { get; private set; }

        public CancellationToken ReceivedCancellationToken { get; private set; }

        public Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto, CancellationToken cancellationToken = default)
        {
            ReceivedUserId = userId;

            ReceivedRequest = changePasswordDto;

            ReceivedCancellationToken = cancellationToken;

            return Task.FromResult(ChangeResult);
        }

        public Task<IReadOnlyCollection<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<UserDto?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<UserDto> CreateAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<UserDto?> UpdateAsync(string userId, UpdateUserDto updateUserDto, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<UserDataLoginDto> GetDataLoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();
    }
}
