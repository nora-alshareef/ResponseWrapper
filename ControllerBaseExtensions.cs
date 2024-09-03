using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ResponseWrapper;

public static class ControllerBaseExtensions
{
    private static IActionResult ApiResponse(this ControllerBase controller, ApiResult result, int statusCode)
    {
        return new ObjectResult(result)
        {
            StatusCode = statusCode
        };
    }
    public static IActionResult ApiResponse(ApiResult result, int statusCode)
    {
        return new ObjectResult(result)
        {
            StatusCode = statusCode
        };
    }
    public static IActionResult ApiResponse(this ControllerBase controller,ApiResult result)
    {
        return new ObjectResult(result)
        {
            StatusCode = ExtensionsUtil.GetStatusCode(result.Status)
        };
    }

    public static IActionResult Success<T>(this ControllerBase controller, T data)
    {
        return controller.ApiResponse(ApiResult.Success(data, controller.HttpContext.TraceIdentifier), 200);
    }

    public static IActionResult BusinessError<TCode>(this ControllerBase controller, TCode code, string message)
    {
        return controller.ApiResponse(ApiResult.BusinessError(code, message, controller.HttpContext.TraceIdentifier), 400);
    }

    public static IActionResult AuthorizationError<TCode>(this ControllerBase controller, TCode code, string message)
    {
        return controller.ApiResponse(ApiResult.AuthorizationError(code, message, controller.HttpContext.TraceIdentifier), 401);
    }

    public static IActionResult ValidationError(this ControllerBase controller, Dictionary<string, string[]> errors)
    {
        return controller.ApiResponse(ApiResult.ValidationError(errors, controller.HttpContext.TraceIdentifier), 400);
    }

    public static IActionResult ServerError<TCode>(this ControllerBase controller, TCode code, string message)
    {
        return controller.ApiResponse(ApiResult.ServerError(code, message, controller.HttpContext.TraceIdentifier), 500);
    }
}


public static class MiddlewareResponseExtensions
{
    public static async Task WriteApiResultAsync(this HttpContext context, ApiResult apiResult)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ExtensionsUtil.GetStatusCode(apiResult.Status);

        await context.Response.WriteAsync(JsonSerializer.Serialize(apiResult));
    }
}

internal static class ExtensionsUtil
{
    internal static int GetStatusCode(string status)
    {
        return status switch
        {
            "success" => StatusCodes.Status200OK,
            "invalid_request" => StatusCodes.Status400BadRequest,
            "authorization_error" => StatusCodes.Status403Forbidden,
            "validation_error" => StatusCodes.Status400BadRequest,
            "server_error" => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError,
        };
    }
}
