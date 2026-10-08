using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using TheBand.AuthApi.Middleware;
using TheBand.AuthApplication.Exceptions;

namespace TheBand.AuthTests;

public sealed class ErrorHandlingMiddlewareTests
{
    [Theory]

    [InlineData("A senha atual está errada.")]

    [InlineData("As senhas não batem.")]
    public async Task Invoke_BusinessException_ReturnsReadableAccents(string message)
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => throw new BusinessException(message),
            NullLogger<ErrorHandlingMiddleware>.Instance);

        var context = new DefaultHttpContext();

        using var responseStream = new MemoryStream();

        context.Response.Body = responseStream;

        await middleware.Invoke(context);

        responseStream.Position = 0;

        using var reader = new StreamReader(responseStream);

        var json = await reader.ReadToEndAsync();

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        Assert.Equal("application/json", context.Response.ContentType);

        Assert.Contains(message, json);

        using var document = JsonDocument.Parse(json);

        Assert.False(document.RootElement.GetProperty("success").GetBoolean());

        Assert.Equal(message, document.RootElement.GetProperty("errors").GetString());
    }
}
