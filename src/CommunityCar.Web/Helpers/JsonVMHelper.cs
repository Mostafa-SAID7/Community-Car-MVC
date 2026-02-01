using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Helpers;

/// <summary>
/// Helper class for JSON ViewModel responses
/// </summary>
public static class JsonVMHelper
{
    /// <summary>
    /// Creates a JSON response for validation errors
    /// </summary>
    /// <param name="errors">Dictionary of validation errors</param>
    /// <returns>JsonResult with validation errors</returns>
    public static JsonResult ValidationError(Dictionary<string, string[]> errors)
    {
        return new JsonResult(new
        {
            success = false,
            message = "Validation failed",
            errors = errors
        })
        {
            StatusCode = 400
        };
    }

    /// <summary>
    /// Creates a JSON response for validation errors with custom message
    /// </summary>
    /// <param name="errors">Dictionary of validation errors</param>
    /// <param name="message">Custom error message</param>
    /// <returns>JsonResult with validation errors</returns>
    public static JsonResult ValidationError(Dictionary<string, string[]> errors, string message)
    {
        return new JsonResult(new
        {
            success = false,
            message = message,
            errors = errors
        })
        {
            StatusCode = 400
        };
    }

    /// <summary>
    /// Creates a JSON success response
    /// </summary>
    /// <param name="data">Response data</param>
    /// <param name="message">Success message</param>
    /// <returns>JsonResult with success response</returns>
    public static JsonResult Success(object? data = null, string message = "Success")
    {
        return new JsonResult(new
        {
            success = true,
            message = message,
            data = data
        });
    }

    /// <summary>
    /// Creates a JSON error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>JsonResult with error response</returns>
    public static JsonResult Error(string message, int statusCode = 500)
    {
        return new JsonResult(new
        {
            success = false,
            message = message
        })
        {
            StatusCode = statusCode
        };
    }
}