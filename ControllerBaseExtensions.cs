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
            StatusCode = GetStatusCode(result)
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
    private static int GetStatusCode(ApiResult result)
    {
        return result.Status switch
        {
            "success" => StatusCodes.Status200OK,
            "invalid_request" => StatusCodes.Status400BadRequest,
            "authorization_error" => StatusCodes.Status401Unauthorized,
            "validation_error" => StatusCodes.Status400BadRequest,
            "server_error" => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError // Default case for unknown status
        };
    }
}