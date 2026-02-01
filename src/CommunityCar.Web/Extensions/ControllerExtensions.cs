using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CommunityCar.Web.Extensions;

/// <summary>
/// Extension methods for Controller to simplify common operations
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Returns JSON result with success status and data
    /// </summary>
    public static JsonResult JsonSuccess(this Controller controller, object? data = null, string? message = null)
    {
        var result = new
        {
            success = true,
            message = message,
            data = data
        };

        return controller.Json(result);
    }

    /// <summary>
    /// Returns JSON result with error status and message
    /// </summary>
    public static JsonResult JsonError(this Controller controller, string message, object? errors = null)
    {
        var result = new
        {
            success = false,
            message = message,
            errors = errors
        };

        return controller.Json(result);
    }

    /// <summary>
    /// Returns JSON result with validation errors
    /// </summary>
    public static JsonResult JsonValidationError(this Controller controller, string message = "Validation failed")
    {
        var errors = controller.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        return controller.JsonError(message, errors);
    }

    /// <summary>
    /// Gets the current user ID from claims
    /// </summary>
    public static Guid? GetCurrentUserId(this Controller controller)
    {
        return controller.User.GetUserId();
    }

    /// <summary>
    /// Gets the current user name from claims
    /// </summary>
    public static string? GetCurrentUserName(this Controller controller)
    {
        return controller.User.GetUserName();
    }

    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    public static bool IsUserAuthenticated(this Controller controller)
    {
        return controller.User.IsAuthenticated();
    }

    /// <summary>
    /// Returns appropriate result based on request type (AJAX vs regular)
    /// </summary>
    public static IActionResult AjaxOrRedirect(this Controller controller, 
        string redirectAction, 
        string? redirectController = null, 
        object? routeValues = null,
        object? ajaxData = null,
        string? ajaxMessage = null)
    {
        if (controller.Request.IsAjaxRequest())
        {
            return controller.JsonSuccess(ajaxData, ajaxMessage);
        }

        if (redirectController != null)
        {
            return controller.RedirectToAction(redirectAction, redirectController, routeValues);
        }

        return controller.RedirectToAction(redirectAction, routeValues);
    }

    /// <summary>
    /// Returns appropriate error result based on request type
    /// </summary>
    public static IActionResult AjaxOrRedirectError(this Controller controller,
        string errorMessage,
        string redirectAction,
        string? redirectController = null,
        object? routeValues = null)
    {
        if (controller.Request.IsAjaxRequest())
        {
            return controller.JsonError(errorMessage);
        }

        controller.TempData["ErrorMessage"] = errorMessage;

        if (redirectController != null)
        {
            return controller.RedirectToAction(redirectAction, redirectController, routeValues);
        }

        return controller.RedirectToAction(redirectAction, routeValues);
    }

    /// <summary>
    /// Sets a success message in TempData
    /// </summary>
    public static void SetSuccessMessage(this Controller controller, string message)
    {
        controller.TempData["SuccessMessage"] = message;
    }

    /// <summary>
    /// Sets an error message in TempData
    /// </summary>
    public static void SetErrorMessage(this Controller controller, string message)
    {
        controller.TempData["ErrorMessage"] = message;
    }

    /// <summary>
    /// Sets an info message in TempData
    /// </summary>
    public static void SetInfoMessage(this Controller controller, string message)
    {
        controller.TempData["InfoMessage"] = message;
    }

    /// <summary>
    /// Sets a warning message in TempData
    /// </summary>
    public static void SetWarningMessage(this Controller controller, string message)
    {
        controller.TempData["WarningMessage"] = message;
    }

    /// <summary>
    /// Returns a view or partial view based on request type
    /// </summary>
    public static IActionResult ViewOrPartial(this Controller controller, string viewName, object? model = null)
    {
        if (controller.Request.IsAjaxRequest())
        {
            return controller.PartialView(viewName, model);
        }

        return controller.View(viewName, model);
    }

    /// <summary>
    /// Serializes an object to JSON string with camelCase naming
    /// </summary>
    public static string ToJson(this Controller controller, object obj)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        return JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// Gets the client IP address
    /// </summary>
    public static string? GetClientIpAddress(this Controller controller)
    {
        return controller.Request.GetClientIpAddress();
    }

    /// <summary>
    /// Checks if the request is from a mobile device
    /// </summary>
    public static bool IsMobileRequest(this Controller controller)
    {
        return controller.Request.IsMobileRequest();
    }

    /// <summary>
    /// Gets the full URL of the current request
    /// </summary>
    public static string GetCurrentUrl(this Controller controller)
    {
        return controller.Request.GetFullUrl();
    }

    /// <summary>
    /// Redirects to the previous page or a default action
    /// </summary>
    public static IActionResult RedirectToPrevious(this Controller controller, 
        string defaultAction, 
        string? defaultController = null)
    {
        var referer = controller.Request.GetReferer();
        
        if (!string.IsNullOrEmpty(referer) && Uri.TryCreate(referer, UriKind.Absolute, out var uri))
        {
            // Only redirect to same host for security
            if (uri.Host == controller.Request.Host.Host)
            {
                return controller.Redirect(referer);
            }
        }

        if (defaultController != null)
        {
            return controller.RedirectToAction(defaultAction, defaultController);
        }

        return controller.RedirectToAction(defaultAction);
    }
}