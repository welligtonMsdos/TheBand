using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TheBand.CoreApplication.Dtos;

namespace TheBand.CoreApi.Middleware;

public sealed class VinylValidationMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly RequestDelegate _next;

    public VinylValidationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(
        HttpContext context,
        IValidator<CreateVinylDto> createValidator,
        IValidator<UpdateVinylDto> updateValidator)
    {
        if (HttpMethods.IsPost(context.Request.Method) && context.Request.Path.Equals("/api/vinyls"))
        {
            var request = await DeserializeAsync<CreateVinylDto>(context);
            if (request is null || !await IsValidAsync(context, request, createValidator)) return;
        }
        else if (HttpMethods.IsPut(context.Request.Method) && context.Request.Path.StartsWithSegments("/api/vinyls/"))
        {
            var request = await DeserializeAsync<UpdateVinylDto>(context);
            if (request is null || !await IsValidAsync(context, request, updateValidator)) return;
        }

        await _next(context);
    }

    private static async Task<TRequest?> DeserializeAsync<TRequest>(HttpContext context)
    {
        try
        {
            context.Request.EnableBuffering();
            var request = await JsonSerializer.DeserializeAsync<TRequest>(context.Request.Body, JsonOptions, context.RequestAborted);
            context.Request.Body.Position = 0;
            if (request is null)
            {
                await WriteProblemAsync(context, new Dictionary<string, string[]> { ["body"] = ["O corpo da requisição é obrigatório."] });
            }
            return request;
        }
        catch (JsonException)
        {
            context.Request.Body.Position = 0;
            await WriteProblemAsync(context, new Dictionary<string, string[]> { ["body"] = ["O corpo da requisição contém JSON inválido."] });
            return default;
        }
    }

    private static async Task<bool> IsValidAsync<TRequest>(HttpContext context, TRequest request, IValidator<TRequest> validator)
    {
        var result = await validator.ValidateAsync(request, context.RequestAborted);
        if (result.IsValid) return true;

        var errors = result.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());
        await WriteProblemAsync(context, errors);
        return false;
    }

    private static Task WriteProblemAsync(HttpContext context, IDictionary<string, string[]> errors)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return context.Response.WriteAsJsonAsync(new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "A requisição contém campos inválidos.",
            Instance = context.Request.Path
        }, context.RequestAborted);
    }
}
