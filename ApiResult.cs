﻿using System.Text.Json.Serialization;
namespace ResponseWrapper
{
    public class ApiResult<T>
    {
        
        public string Status { get; set; }
        public string TraceId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string[]> Errors { get; set; }

        public static ApiResult<T> Success(T data, string traceId)
        {
            return new ApiResult<T>
            {
                Status = "Success",
                TraceId = traceId,
                Data = data
            };
        }

        public static ApiResult<T> BusinessError<TCode>(TCode code, string message, string traceId)
        {
            string convertedCode = ConvertToString(code);
            return new ApiResult<T>
            {
                Status = "Error",
                TraceId = traceId,
                Errors = new Dictionary<string, string[]> { { convertedCode, new[] { message } } }
            };
        }

        public static ApiResult<T> ValidationError(Dictionary<string, string[]> errors, string traceId)
        {
            return new ApiResult<T>
            {
                Status = "Error",
                TraceId = traceId,
                Errors = errors
            };
        }
    
        public static ApiResult<T> AuthorizationError<TCode>(TCode code, string message, string traceId)
        {
            string convertedCode = ConvertToString(code);
            return new ApiResult<T>
            {
                Status = "Error",
                TraceId = traceId,
                Errors = new Dictionary<string, string[]> { { convertedCode, new[] { message } } }
            };
        }
    
        public static ApiResult<T> ServerError<TCode>(TCode code, string message, string traceId)
        {
            string convertedCode = ConvertToString(code);
            return new ApiResult<T>
            {
                Status = "Error",
                TraceId = traceId,
                Errors = new Dictionary<string, string[]> { { convertedCode, new[] { message } } }
            };
        }

        private static string ConvertToString<TCode>(TCode code)
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
    }
    
}

