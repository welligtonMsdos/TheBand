using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TheBand.AuthApi.Middleware;

namespace TheBand.AuthApi.Filters;

public class ValidatorFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidatorFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

            var validator = _serviceProvider.GetService(validatorType) as IValidator;

            if (validator != null)
            {
                var validationContext = new ValidationContext<object>(argument);

                var validationResult = await validator.ValidateAsync(validationContext);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );

                    context.Result = new BadRequestObjectResult(Result<object>.Failure(errors));

                    return;
                }
            }
        }

        if (context.ActionArguments.Any(x => x.Value == null))
        {
            context.Result = new BadRequestObjectResult(Result<object>.Failure("O corpo da requisição não pode ser vazio."));

            return;
        }

        await next();
    }
}
