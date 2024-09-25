# Changelog

All notable changes to this project will be documented in this file.
### Added 
You have 2 options regarding ConfigureInvalidModelStateResponse : 
1- including Func<ActionContext, Dictionary<string, string[]>> as more validation input to be taken from other Annotations/validation libraries
So if you want to keep your current validation or add extra , you can do that like here:
.ConfigureApiBehaviorOptions(options =>
ApiBehaviorConfigurator.ConfigureInvalidModelStateResponse(
options,
CustomErrorFactory.InvalidModelStateResponseFactoryWithCodes
)

or keep the default one like here :
.ConfigureApiBehaviorOptions(options =>
ApiBehaviorConfigurator.ConfigureInvalidModelStateResponse()

2- (internal to idart) .. always do the first one with usie of Annotations Utils.. the thing is : CustomErrorFactory.InvalidModelStateResponseFactoryWithCodes
will handle all the types of annotations even the dotnet ones or external custom attributes.


## [1.0.0] - 2024-09-02

### Added
- Initial release of the ResponseWrapper library for .NET 8.
- Standardized response structure for API endpoints:
  - Success responses
  - Validation error responses
  - Business error responses
  - Authorization error responses
  - Server error responses
- Extension methods for controllers to facilitate response handling:
  - `Success<T>(T data)`
  - `BusinessError<TCode>(TCode code, string message)`
  - `ValidationError(Dictionary<string, string[]> errors)`
  - `AuthorizationError<TCode>(TCode code, string message)`
  - `ServerError<TCode>(TCode code, string message)`
- Automatic handling of model validations and exceptions.

### Changed
- Improved documentation with examples for response structure and usage.

### Fixed
- No known issues fixed in this release.

---

## Future Releases

- Planned enhancements for additional response types and improved error handling.