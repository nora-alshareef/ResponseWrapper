using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ResponseWrapper;

public static class ApiBehaviorConfigurator
{
    public static void ConfigureInvalidModelStateResponse(ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var result = ApiResult<object>.ValidationError(errors, context.HttpContext.TraceIdentifier);
            return ControllerBaseExtensions.ApiResponse(result, 400);
        };
    }
}