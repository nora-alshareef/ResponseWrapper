using Microsoft.AspNetCore.Mvc;

namespace ResponseWrapper;

public static class ControllerBaseExtensions
{
    private static IActionResult ApiResponse<T>(this ControllerBase controller, ApiResult<T> result, int statusCode)
    {
        return new ObjectResult(result)
        {
            StatusCode = statusCode
        };
    }
    public static IActionResult ApiResponse<T>(ApiResult<T> result, int statusCode)
    {
        return new ObjectResult(result)
        {
            StatusCode = statusCode
        };
    }

    public static IActionResult Success<T>(this ControllerBase controller, T data)
    {
        return controller.ApiResponse(ApiResult<T>.Success(data, controller.HttpContext.TraceIdentifier), 200);
    }

    public static IActionResult BusinessError<TCode>(this ControllerBase controller, TCode code, string message)
    {
        return controller.ApiResponse(ApiResult<object>.BusinessError(code, message, controller.HttpContext.TraceIdentifier), 400);
    }

    public static IActionResult AuthorizationError<TCode>(this ControllerBase controller, TCode code, string message)
    {
        return controller.ApiResponse(ApiResult<object>.AuthorizationError(code, message, controller.HttpContext.TraceIdentifier), 401);
    }

    public static IActionResult ValidationError(this ControllerBase controller, Dictionary<string, string[]> errors)
    {
        return controller.ApiResponse(ApiResult<object>.ValidationError(errors, controller.HttpContext.TraceIdentifier), 400);
    }

    public static IActionResult ServerError<TCode>(this ControllerBase controller, TCode code, string message)
    {
        return controller.ApiResponse(ApiResult<object>.ServerError(code, message, controller.HttpContext.TraceIdentifier), 500);
    }
}