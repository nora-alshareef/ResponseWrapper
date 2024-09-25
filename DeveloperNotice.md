## Developer notice 

Note : You don't need to read this file unless you want to review the library or contribute to building it

This library is responsible for many scenarios:

### ServerError
1. When an exception occurs, it will manage the response to be structured. We were using filters, but now we use middleware. This is because we did not want to use any middleware unless we truly needed it. However, we have other scenarios that require middleware, so we stopped using filters.

### ProtocolError
2. When the client makes a mistake that does not result in a 400 error, such as 405, 415, 404, etc. Any status code that is not 200, 400, 403, or 500 can trigger these scenarios. To handle these cases, we needed to create middleware. However, .NET uses its own mapper, so even with middleware, it will not work unless you add this line: 
   ```csharp
   ApiBehaviorOptions.SuppressMapClientErrors = true;
   ```
   This is the way to tell .NET to stop using its own mapping. This is only recommended if you know what you are doing. The codes are taken from the status code to ensure unique and full mapping:
   ```json
   {
     "traceId": "0HN6HR0TN1OPK:00000001",
     "error": {
       "code": "P404",
       "message": "Not Found"
     }
   }
   ```

### ValidationError
3. When a validation error occurs, this library triggers it and returns the proper structure. We trigger it using this line:
   ```csharp
   .ConfigureApiBehaviorOptions(ApiBehaviorConfigurator.ConfigureInvalidModelStateResponse)
   ```
   While we wanted to include it in the middleware, unfortunately, the middleware does not have access to `ActionContext`. We also wanted to pass `ActionContext` to middleware, but middleware is designed to process requests and responses without specific knowledge of the current action being executed. Thus, that was not a viable option.

### BusinessError and AuthorizationError
4. Other errors, like business errors and authorization errors, are returned directly after being structured.

### Success
5. A success scenario is returned after being structured.

---------------------

Now we have other scenarios depending on where the user can use this library:

1. **From some middleware**: If the user has authentication middleware and wants to return an auth error before the request triggers the controller, they just need this line in their middleware:
   ```csharp
   // Define the type of error
   var result = ApiResult.AuthorizationError("A002", "Secret is wrong", context.TraceIdentifier);
   // Then write it
   await context.WriteApiResultAsync(result);
   ```

2. **From a controller**: The user just needs this line:
   ```csharp
   return this.Success(userDto.Name);
   ```
   or 
   ```csharp
   return this.BusinessError(userDto.Name);
   ```
   or anything else.

3. **From another class** (please, if you do that, do it from the BL class):
   ```csharp
   return ApiResult.BusinessError("code", "message", traceId);
   ```
   But also, you need to return `ApiResponse` from the controller because you don’t know which `ApiResult` has been returned from the BL:
   ```csharp
   var apiResult = await bl.Method(requestModel.Property, traceId);
   return this.ApiResponse(apiResult);
   ```