# Changelog

All notable changes to this project will be documented in this file.

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