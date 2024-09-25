using System.Text.Json.Serialization;
namespace ResponseWrapper
{
    public record Error(string Code, string? Message);

    public class ApiResult
    {
        public string? TraceId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Error? Error { get; set; } // Use object for generic compatibility

        // Static method to convert code to string
        private static string ConvertToString<TCode>(TCode code)
        {
            if (code is not null)
            {
                if (code is Enum)
                {
                    return (Enum.GetName(code.GetType(), code) ?? code.ToString()) ??
                           throw new InvalidOperationException();
                } 
                if (code is string or int or char)
                {
                    return code.ToString();
                }
            }
            
            throw new ArgumentException($"Unsupported type for error code: {typeof(TCode).Name}", nameof(code));
            
        }
        
        public static Success CreateSuccess(object? data, string traceId)
        {
            return new Success(data, traceId);
        }

        // Static factory methods for creating error instances
        public static ServerError CreateServerError<TCode>(TCode code, string message, string traceId)
        {
            var codeStr = ConvertToString(code);
            if (codeStr.Length > 0 && char.ToUpper(codeStr[0]) != 'S')
                throw new ArgumentException("Code does not start with S or s.");

            return new ServerError(codeStr, message, traceId);
        }
        public static ProtocolError CreateProtocolError<TCode>(TCode code, string? message, string traceId)
        {
            var codeStr = ConvertToString(code);
            return new ProtocolError(codeStr, message, traceId);
        }

        public static BusinessError CreateBusinessError<TCode>(TCode code, string? message, string traceId)
        {
            var codeStr = ConvertToString(code);
            return new BusinessError(codeStr, message, traceId);
        }

        public static ValidationError CreateValidationError(Dictionary<string, string[]> errors, string traceId)
        {
            var (code, message) =HandleValidationDictionary(errors);
            return new ValidationError(code,message, traceId);
        }

        public static ValidationError CreateValidationError<TCode>(TCode code,string message, string traceId)
        {
            var codeStr = ConvertToString(code);
            return new ValidationError(codeStr,message, traceId);
        }

        public static AuthorizationError CreateAuthorizationError<TCode>(TCode code, string message, string traceId)
        {
            var codeStr = ConvertToString(code);

            return new AuthorizationError(codeStr, message, traceId);
        }

        private static (string,string) HandleValidationDictionary(Dictionary<string, string[]> errors)
        {
            var errorString = errors.FirstOrDefault().Value[0] ;
            
            // Check if the errorString starts with V followed by 3 digits and a colon
            if (errorString.Length > 5 && 
                errorString[0] == 'V' && 
                char.IsDigit(errorString[1]) && 
                char.IsDigit(errorString[2]) && 
                char.IsDigit(errorString[3]) && 
                errorString[4] == ':')
            {
                string[] parts = errorString.Split(':', 2);
                var code = parts[0];
                var message = parts.Length > 1 ? parts[1].Trim() : "";
                return (code, message);
            }
            else
            {
                string[] parts = errorString.Split('.', 2);
                var summeryError = parts[0];
                return ("V999", summeryError);
            }
        }
    }

    public class Success : ApiResult
    {
        public Success(object? data, string traceId)
        {
            Data = data;
            TraceId = traceId;
        }
    }

    public class BusinessError : ApiResult
    {
        internal BusinessError(string code, string? message, string traceId)
        {
            if (code.Length > 0 && char.ToUpper(code[0]) != 'B')
                throw new ArgumentException("Code does not start with B or b.");
            TraceId = traceId;
            Error = new Error(code, message);
        }
    }

    public class ValidationError : ApiResult
    {
        public ValidationError(string code, string message, string traceId)
        {
            TraceId = traceId;
            Error = new Error(code, message);
        }
    }

    public class AuthorizationError : ApiResult
    {
        public AuthorizationError(string code, string message, string traceId)
        {
            if (code.Length > 0 && char.ToUpper(code[0]) != 'A')
                throw new ArgumentException("Code does not start with A or a.");
            
            TraceId = traceId;
            Error = new Error(code, message);
        }
    }

    public class ServerError : ApiResult
    {
        internal ServerError(string code, string message, string traceId)
        {
            if (code.Length > 0 && char.ToUpper(code[0]) != 'S')
                throw new ArgumentException("Code does not start with S or s.");
            
            TraceId = traceId;
            Error = new Error(code, message);
        }
    }
    
    public class ProtocolError : ApiResult
    {
        internal ProtocolError(string code, string? message, string traceId)
        {
            if (code.Length > 0 && char.ToUpper(code[0]) != 'P')
                throw new ArgumentException("Code does not start with P or p.");
            
            TraceId = traceId;
            Error = new Error(code, message);
        }
    }
}