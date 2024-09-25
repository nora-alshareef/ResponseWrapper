using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ResponseWrapper;

public class ResponseWrapperMiddleware(
    RequestDelegate next,
    ILogger<ResponseWrapperMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context);
            return;
        }
        if (Utils.IsExcludedStatusCode(context.Response.StatusCode))
        {
            await HandleExcludedStatusCodesAsync(context);
        }
    }
    
    private static async Task HandleExceptionAsync(HttpContext context)
    {
        var result = ApiResult.CreateServerError("S001", "Unhandled exception", context.TraceIdentifier);
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.WriteApiResultAsync(result);
    }
    
    private static async Task HandleExcludedStatusCodesAsync(HttpContext context)
    {
        var statusCode = context.Response.StatusCode;
        var description = Utils.GetDescription(statusCode);
        var result = ApiResult.CreateProtocolError($"P{statusCode}",
            description, context.TraceIdentifier);
        await context.WriteApiResultAsync(result);
    }
}
