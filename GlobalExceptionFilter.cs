using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ResponseWrapper;

public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, "An unhandled exception has occurred.");

        var result = ApiResult<object>.ServerError("S001", "unhandled exception", context.HttpContext.TraceIdentifier);

        context.Result = new ObjectResult(result)
        {
            StatusCode = 500
        };
        context.ExceptionHandled = true;
    }
}