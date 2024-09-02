using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace ResponseWrapper
{
    public class ApiResult
    {
        public required string Status { get; set; }
        public required string TraceId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string[]>? Errors { get; set; }

        public static ApiResult Success<T>(T data, string traceId)
        {
            return new ApiResult
            {
                Status = "success",
                TraceId = traceId,
                Data = data
            };
        }

        public static ApiResult BusinessError<TCode>(TCode code, string message, string traceId)
        {
            var convertedCode = ConvertToString(code);
            return new ApiResult
            {
                Status = "invalid_request",
                TraceId = traceId,
                Errors = new Dictionary<string, string[]> { { convertedCode??string.Empty, new[] { message } } }
            };
        }

        public static ApiResult ValidationError(Dictionary<string, string[]> errors, string traceId)
        {
            return new ApiResult
            {
                Status = "validation_error",
                TraceId = traceId,
                Errors = errors
            };
        }
    
        public static ApiResult AuthorizationError<TCode>(TCode code, string message, string traceId)
        {
            string? convertedCode = ConvertToString(code);
            return new ApiResult
            {
                Status = "authorization_error",
                TraceId = traceId,
                Errors = new Dictionary<string, string[]> { {  convertedCode??string.Empty, new[] { message } } }
            };
        }
        
        
    
        public static ApiResult ServerError<TCode>(TCode code, string message, string traceId)
        {
            var convertedCode = ConvertToString(code);
            return new ApiResult
            {
                Status = "server_error",
                TraceId = traceId,
                Errors = new Dictionary<string, string[]> { {  convertedCode??string.Empty, new[] { message } } }
            };
        }
        

        private static string? ConvertToString<TCode>(TCode code)
        {
            if (code is Enum)
            {
                return Enum.GetName(code.GetType(), code) ?? code.ToString();
            }
            else if (code is string || code is int || code is char)
            {
                return code.ToString();
            }
            else
            {
                throw new ArgumentException($"Unsupported type for error code: {typeof(TCode).Name}", nameof(code));
            }
        }
        
        // New method to get the HTTP status code based on ApiResult status
        
    }
    
}

