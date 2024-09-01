# ResponseWrapper for .NET 8

This ResponseWrapper library provides a standardized way to structure API responses in .NET 8 applications. It helps maintain consistency across your API endpoints and makes it easier to handle responses on the client side.

## Installation

### Build and Use the Library

Clone and build the repository:
git clone https://github.com/yourusername/ResponseWrapper.git

```shell

Build and pack:
cd ResponseWrapper
dotnet build
dotnet pack
```

### Add Reference to the ResponseWrapper DLL

To add a reference to the ResponseWrapper DLL in your project, follow these steps:

1. Right-click on your project in the Solution Explorer.
2. Select "Add" > "Reference".
3. In the Reference Manager dialog, click on "Browse".
4. Navigate to the location of the ResponseWrapper DLL file.
5. Select the DLL file (e.g., "ResponseWrapper.dll") and click "Add".
6. Click "OK" in the Reference Manager dialog to confirm.

Alternatively, if you're using the command line or prefer editing the .csproj file directly, you can add the following line within an `<ItemGroup>` in your project file:

```xml
<Reference Include="ResponseWrapper">
    <HintPath>path\to\ResponseWrapper.dll</HintPath>
</Reference>
```
Setup
In your Program.cs file, configure your application to use ResponseWrapper:
```csharp

using ResponseWrapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    {
        options.Filters.Add<GlobalExceptionFilter>();
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    }).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
```

Usage
Returning Responses
Use the extension methods provided by ResponseWrapper in your controllers:
```csharp
public class ExampleController : ControllerBase
{
[HttpGet]
public IActionResult Get()
{
var data = new { Message = "Hello, World!" };
return this.Success(data);
}

    [HttpPost]
    public IActionResult Post([FromBody] SomeModel model)
    {
        if (!ModelState.IsValid)
        {
            return this.ValidationError(ModelState.ToDictionary());
        }

        // Process the model...

        return this.Success(new { Id = 1 });
    }

    [HttpGet("error")]
    public IActionResult GetError()
    {
        return this.BusinessError("ERR001", "An error occurred");
    }
}
```
## Response Structure
The standard response structure is:
```
{
"status": "Success" | "Error",
"traceId": "unique-trace-id",
"data": { ... },  // Only for successful responses
"errors": { ... }  // Only for error responses
}
```
### Available Methods

The ResponseWrapper provides several methods to create different types of responses:

1. `Success<T>(T data)`: Returns a success response with data.
2. `BusinessError<TCode>(TCode code, string message)`: Returns a business error response.
3. `ValidationError(Dictionary<string, string[]> errors)`: Returns a validation error response.
4. `AuthorizationError<TCode>(TCode code, string message)`: Returns an authorization error response.
5. `ServerError<TCode>(TCode code, string message)`: Returns a server error response.

### Accepted Types for TCode

The `TCode` generic parameter in methods like `BusinessError`, `AuthorizationError`, and `ServerError` allows for flexibility in error code representation. The accepted types for `TCode` are:

- `int`: For numeric error codes.
- `string`: For string-based error codes.
- `Enum`: For enum-based error codes.

This flexibility allows you to use the error code format that best fits your application's needs.

## Automatic Handling of Validations and Exceptions

The ResponseWrapper is designed to automatically handle model validations and exceptions, ensuring that all responses maintain a consistent structure.

### Validation Annotations

When you use validation annotations on your model properties, such as `[Required]`, `[StringLength]`, `[Range]`, etc., the ResponseWrapper will automatically catch these validation errors and return them in a structured format.

Example of a model with validation annotations:
```csharp
public class UserDto
{
    [Required]
    public string Name { get; set; }

    [EmailAddress]
    public string Email { get; set; }
}
```

Example of Validation Error Response
```json

{
"success": false,
"message": "Validation failed",
"errors": {
"name": ["The Name field is required."],
"email": ["The Email field is not a valid e-mail address."]
}
}
```
Business Error Response
```json

{
"success": false,
"message": "User not found",
"errorCode": "USER_NOT_FOUND"
}
```

Server Error Response
```json
{
"success": false,
"message": "An unexpected error occurred",
"errorCode": "INTERNAL_SERVER_ERROR"
}
```