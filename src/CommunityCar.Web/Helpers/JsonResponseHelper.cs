using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CommunityCar.Web.Helpers;

/// <summary>
/// Helper class for creating consistent JSON responses
/// </summary>
public static class JsonResponseHelper
{
    /// <summary>
    /// Creates a standardized success JSON response
    /// </summary>
    public static JsonResult Success(string? message = null, object? data = null)
    {
        var response = new
        {
            success = true,
            message = message ?? "Operation completed successfully",
            data = data,
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response);
    }

    /// <summary>
    /// Creates a standardized error JSON response
    /// </summary>
    public static JsonResult Error(string message, object? errors = null, int statusCode = 400)
    {
        var response = new
        {
            success = false,
            message = message,
            errors = errors,
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response) { StatusCode = statusCode };
    }

    /// <summary>
    /// Creates a validation error JSON response
    /// </summary>
    public static JsonResult ValidationError(Dictionary<string, string[]> validationErrors)
    {
        var response = new
        {
            success = false,
            message = "Validation failed",
            errors = validationErrors,
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response) { StatusCode = 422 };
    }

    /// <summary>
    /// Creates a paginated data JSON response
    /// </summary>
    public static JsonResult PaginatedData<T>(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        var response = new
        {
            success = true,
            data = items,
            pagination = new
            {
                currentPage = currentPage,
                pageSize = pageSize,
                totalCount = totalCount,
                totalPages = totalPages,
                hasNextPage = currentPage < totalPages,
                hasPreviousPage = currentPage > 1
            },
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response);
    }

    /// <summary>
    /// Creates a JSON response for authentication scenarios
    /// </summary>
    public static JsonResult AuthenticationRequired(string? redirectUrl = null)
    {
        var response = new
        {
            success = false,
            message = "Authentication required",
            requiresAuth = true,
            redirectUrl = redirectUrl,
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response) { StatusCode = 401 };
    }

    /// <summary>
    /// Creates a JSON response for authorization failures
    /// </summary>
    public static JsonResult Forbidden(string message = "Access denied")
    {
        var response = new
        {
            success = false,
            message = message,
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response) { StatusCode = 403 };
    }

    /// <summary>
    /// Creates a JSON response for not found scenarios
    /// </summary>
    public static JsonResult NotFound(string message = "Resource not found")
    {
        var response = new
        {
            success = false,
            message = message,
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response) { StatusCode = 404 };
    }

    /// <summary>
    /// Creates a JSON response for server errors
    /// </summary>
    public static JsonResult ServerError(string message = "An internal server error occurred")
    {
        var response = new
        {
            success = false,
            message = message,
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response) { StatusCode = 500 };
    }

    /// <summary>
    /// Creates a JSON response with custom status code
    /// </summary>
    public static JsonResult Custom(bool success, string message, object? data = null, int statusCode = 200)
    {
        var response = new
        {
            success = success,
            message = message,
            data = data,
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response) { StatusCode = statusCode };
    }

    /// <summary>
    /// Creates a JSON response for file upload results
    /// </summary>
    public static JsonResult FileUpload(bool success, string? fileName = null, string? fileUrl = null, string? message = null)
    {
        var response = new
        {
            success = success,
            message = message ?? (success ? "File uploaded successfully" : "File upload failed"),
            data = success ? new { fileName = fileName, fileUrl = fileUrl } : null,
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response);
    }

    /// <summary>
    /// Creates a JSON response for search results
    /// </summary>
    public static JsonResult SearchResults<T>(IEnumerable<T> results, string query, int totalCount, TimeSpan? searchTime = null)
    {
        var response = new
        {
            success = true,
            message = $"Found {totalCount} results for '{query}'",
            data = new
            {
                query = query,
                results = results,
                totalCount = totalCount,
                searchTime = searchTime?.TotalMilliseconds
            },
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response);
    }

    /// <summary>
    /// Creates a JSON response for batch operations
    /// </summary>
    public static JsonResult BatchOperation(int successCount, int failureCount, IEnumerable<string>? errors = null)
    {
        var totalCount = successCount + failureCount;
        var response = new
        {
            success = failureCount == 0,
            message = $"Processed {totalCount} items: {successCount} successful, {failureCount} failed",
            data = new
            {
                totalCount = totalCount,
                successCount = successCount,
                failureCount = failureCount,
                errors = errors
            },
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response);
    }

    /// <summary>
    /// Creates a JSON response with redirect information
    /// </summary>
    public static JsonResult Redirect(string url, string? message = null)
    {
        var response = new
        {
            success = true,
            message = message ?? "Redirecting...",
            redirect = true,
            redirectUrl = url,
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response);
    }

    /// <summary>
    /// Creates a JSON response for partial view content
    /// </summary>
    public static JsonResult PartialView(string html, string? message = null)
    {
        var response = new
        {
            success = true,
            message = message,
            html = html,
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response);
    }

    /// <summary>
    /// Serializes an object to JSON with custom options
    /// </summary>
    public static JsonResult SerializeWithOptions<T>(T data, JsonSerializerOptions? options = null)
    {
        options ??= new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        return new JsonResult(data, options);
    }

    /// <summary>
    /// Creates a JSON response for real-time updates
    /// </summary>
    public static JsonResult RealtimeUpdate(string eventType, object data, string? targetId = null)
    {
        var response = new
        {
            success = true,
            eventType = eventType,
            data = data,
            targetId = targetId,
            timestamp = DateTime.UtcNow
        };

        return new JsonResult(response);
    }
}