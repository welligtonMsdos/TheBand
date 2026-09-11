using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TheBand.CoreApi.Filters;

public sealed class UserIdMatchesTokenFilter : IAsyncActionFilter
{
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userId = context.ActionArguments.TryGetValue("userId", out var value) ? value as string : null;
        var tokenUserId = context.HttpContext.User.FindFirst("id")?.Value;

        if (string.IsNullOrWhiteSpace(userId) || !string.Equals(userId, tokenUserId, StringComparison.Ordinal))
        {
            context.Result = new ForbidResult();

            return Task.CompletedTask;
        }

        return next();
    }
}
