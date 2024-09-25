using Microsoft.AspNetCore.Mvc;

namespace ResponseWrapper;

public static class ApiBehaviorConfigurator
{
        public static void ConfigureInvalidModelStateResponse(ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = new Dictionary<string, string[]>();
                foreach (var e1 in context.ModelState)
                {
                    if (e1.Value is { Errors.Count: > 0 })
                        errors.Add(e1.Key, e1.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                }

                var result = ApiResult.CreateValidationError(errors, context.HttpContext.TraceIdentifier);
                return ControllerExtensions.ApiResponse(result, 400);
            };
        }
        
        public static void ConfigureInvalidModelStateResponse(
            ApiBehaviorOptions options,
            Func<ActionContext, Dictionary<string, string[]>> customErrorFactory)
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = customErrorFactory(context);
                
                var result = ApiResult.CreateValidationError(errors, context.HttpContext.TraceIdentifier);
                return ControllerExtensions.ApiResponse(result, 400);
            };
        }
    }