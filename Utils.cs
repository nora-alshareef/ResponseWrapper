using System.Net;
using System.Text.RegularExpressions;

namespace ResponseWrapper;

internal static class Utils
{
    internal static int GetStatusCode(ApiResult result)
    {
        return result switch
        {
            Success => 200,
            ValidationError => 400,
            BusinessError => 400,
            AuthorizationError => 403,
            ServerError => 500,
            _ => throw new ArgumentException("unknown status code")
        };
    }
    public static bool IsExcludedStatusCode(int statusCode)
    {
        return statusCode != 200 && statusCode != 400 && statusCode != 500 && statusCode != 403;
    }
    
    public static string GetDescription(int statusCode)
    {
        var description = ((HttpStatusCode)statusCode).ToString(); // 404 -> "NotFound"
        return Regex.Replace(description, "([A-Z])", " $1").Trim();
    }
}