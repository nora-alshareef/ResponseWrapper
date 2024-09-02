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
// your configs here ..
builder.Services.AddControllers(options => options.Filters.Add<GlobalExceptionFilter>())
    .ConfigureApiBehaviorOptions(ApiBehaviorConfigurator.ConfigureInvalidModelStateResponse)
    .AddJsonOptions(JsonOptionsConfigurator.ConfigureJsonOptions);
// make sure that configured AddControllers is before AddEndpointsApiExplorer
builder.Services.AddEndpointsApiExplorer();

//other configs here .
var app = builder.Build();
```

Usage
Returning Responses
Use the extension methods provided by ResponseWrapper in your controllers:
```csharp
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ResponseWrapper;

namespace test;

[ApiController]
[Route("[controller]")]
public class ExampleController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var data = new { Message = "Hello, World!" };
        return this.Success(data);
    }

    [HttpGet("businessError")]
    public IActionResult GetBusinessError()
    {
        return this.BusinessError("B001", "An error occurred");
    }
    
    [HttpGet("serverError")]
    public IActionResult GetServerError()
    {
        throw new Exception();
    }
    [HttpPost("validationError")]
    public IActionResult GetValidationError([FromBody] UserDto userDto)
    {
        return this.Success(userDto.Name);
    }
    
    [HttpPost("authError")]
    public IActionResult GetAuthError()
    {
        return this.AuthorizationError("A001","wrong api key");
    }
    public class UserDto
    {
        [Length(50,51)]
        public string? Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
```
## Response Structure
The standard response structure is:
```
{
"status": "success" | "validation_error" | "invalid_request" | "authorization_error" | "server_error"
"traceId": "unique-trace-id",
"data": { ... },  // Only for successful responses
"errors": { ... }  // Only for failure responses
}
```

Example of Validation Error Response
```json

{
  "status": "validation_error",
  "traceId": "0HN6BAV9MLRNU:00000004",
  "errors": {
    "Name": [
      "The field Name must be a string or collection type with a minimum length of '50' and maximum length of '51'."
    ]
  }
}
```
Business Error Response
```json

{
  "status": "invalid_request",
  "traceId": "0HN6BATGGO9G2:00000004",
  "errors": {
    "B001": [
      "An error occurred"
    ]
  }
}
```

Authorization Error Response
```json
{
  "status": "authorization_error",
  "traceId": "0HN6BB31TKENI:00000003",
  "errors": {
    "A001": [
      "wrong api key"
    ]
  }
}
```
Server Error Response
```json
{
  "status": "server_error",
  "traceId": "0HN6BAV9MLRNU:00000003",
  "errors": {
    "S001": [
      "unhandled exception"
    ]
  }
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

* Required: use [ApiController] and [Route("[controller]")] before controller class definition
in order to let checking the validation error goes implicitly

When you use validation annotations on your model properties, such as `[Required]`, `[StringLength]`, `[Range]`, etc., the ResponseWrapper will automatically catch these validation errors and return them in a structured format.
