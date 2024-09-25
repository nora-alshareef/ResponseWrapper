using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ResponseWrapper;

public static class ControllerExtensions
{
    public static IActionResult ApiResponse(this ControllerBase controller,ApiResult result)
    {
        return ApiResponse(controller, result, Utils.GetStatusCode(result)); //if developer use CreateProtocol Error in controller, an exception will occure,
                                                                             //protocolError should not be involved in business or created by developer.
        
    }
    private static IActionResult ApiResponse(this ControllerBase controller, ApiResult result, int statusCode)
    {
        return ApiResponse(result, statusCode);
    }
    
    public static IActionResult ApiResponse(ApiResult result, int statusCode)
    {
        var objectResult = new ObjectResult(result)
        {
            StatusCode = statusCode
        };
        if (statusCode >= 400)
            objectResult.ContentTypes.Add("application/problem+json"); 

        return objectResult;
    }

    public static IActionResult Success<T>(this ControllerBase controller, T? data)
    {
        return controller.ApiResponse(ApiResult.CreateSuccess(data, controller.HttpContext.TraceIdentifier), 200);
    }

    public static IActionResult BusinessError<TCode>(this ControllerBase controller, TCode code, string? message)
    {
        return controller.ApiResponse(ApiResult.CreateBusinessError(code, message, controller.HttpContext.TraceIdentifier), 400);
    }

    public static IActionResult AuthorizationError<TCode>(this ControllerBase controller, TCode code, string message)
    {
        return controller.ApiResponse(ApiResult.CreateAuthorizationError(code, message, controller.HttpContext.TraceIdentifier), 401);
    }

    public static IActionResult ValidationError(this ControllerBase controller, Dictionary<string?, string[]> errors)
    {
        return controller.ApiResponse(ApiResult.CreateValidationError(errors, controller.HttpContext.TraceIdentifier), 400);
    }

    public static IActionResult ServerError<TCode>(this ControllerBase controller, TCode code, string message)
    {
        return controller.ApiResponse(ApiResult.CreateServerError(code, message, controller.HttpContext.TraceIdentifier), 500);
    }
}


public static class MiddlewareResponseExtensions
{
    public static async Task WriteApiResultAsync(this HttpContext context, ApiResult apiResult)
    {
        if (context.Response.StatusCode == 200)
        {
            var statusCode = Utils.GetStatusCode(apiResult);
            context.Response.StatusCode = statusCode;
        }
        if (context.Response.StatusCode >= 400)
            context.Response.ContentType="application/problem+json"; // Set Content-Type
        await context.Response.WriteAsync(JsonSerializer.Serialize(apiResult));
    }
}

