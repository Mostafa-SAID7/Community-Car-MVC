using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CommunityCar.Web.Attributes;

/// <summary>
/// Restricts action to AJAX requests only
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AjaxOnlyAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Custom error message for non-AJAX requests
    /// </summary>
    public string ErrorMessage { get; set; } = "This action only accepts AJAX requests.";

    /// <summary>
    /// HTTP status code to return for non-AJAX requests
    /// </summary>
    public int StatusCode { get; set; } = 400;

    /// <summary>
    /// Whether to redirect non-AJAX requests to a specific action
    /// </summary>
    public string? RedirectAction { get; set; }

    /// <summary>
    /// Controller for redirect (if different from current)
    /// </summary>
    public string? RedirectController { get; set; }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!IsAjaxRequest(context.HttpContext.Request))
        {
            if (!string.IsNullOrEmpty(RedirectAction))
            {
                // Redirect to specified action
                context.Result = new RedirectToActionResult(
                    RedirectAction, 
                    RedirectController ?? context.RouteData.Values["controller"]?.ToString(),
                    null);
            }
            else
            {
                // Return error response
                context.Result = new JsonResult(new { error = ErrorMessage })
                {
                    StatusCode = StatusCode
                };
            }
        }

        base.OnActionExecuting(context);
    }

    private static bool IsAjaxRequest(HttpRequest request)
    {
        return request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
               request.Headers.Accept.Any(a => a?.Contains("application/json") == true) ||
               request.ContentType?.Contains("application/json") == true;
    }
}

/// <summary>
/// Restricts action to non-AJAX requests only (regular HTTP requests)
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class NoAjaxAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Custom error message for AJAX requests
    /// </summary>
    public string ErrorMessage { get; set; } = "This action does not accept AJAX requests.";

    /// <summary>
    /// HTTP status code to return for AJAX requests
    /// </summary>
    public int StatusCode { get; set; } = 400;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (IsAjaxRequest(context.HttpContext.Request))
        {
            context.Result = new JsonResult(new { error = ErrorMessage })
            {
                StatusCode = StatusCode
            };
        }

        base.OnActionExecuting(context);
    }

    private static bool IsAjaxRequest(HttpRequest request)
    {
        return request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
               request.Headers.Accept.Any(a => a?.Contains("application/json") == true) ||
               request.ContentType?.Contains("application/json") == true;
    }
}

/// <summary>
/// Provides different responses for AJAX vs regular requests
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AjaxResponseAttribute : ActionFilterAttribute
{
    /// <summary>
    /// View name for regular requests
    /// </summary>
    public string? ViewName { get; set; }

    /// <summary>
    /// Partial view name for AJAX requests
    /// </summary>
    public string? PartialViewName { get; set; }

    /// <summary>
    /// Whether to return JSON for AJAX requests instead of partial view
    /// </summary>
    public bool ReturnJson { get; set; } = false;

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ViewResult viewResult)
        {
            var isAjaxRequest = IsAjaxRequest(context.HttpContext.Request);

            if (isAjaxRequest)
            {
                if (ReturnJson)
                {
                    // Convert view result to JSON
                    context.Result = new JsonResult(new
                    {
                        success = true,
                        data = viewResult.Model,
                        viewName = viewResult.ViewName ?? PartialViewName
                    });
                }
                else if (!string.IsNullOrEmpty(PartialViewName))
                {
                    // Return partial view for AJAX requests
                    context.Result = new PartialViewResult
                    {
                        ViewName = PartialViewName,
                        ViewData = viewResult.ViewData,
                        TempData = viewResult.TempData
                    };
                }
            }
            else if (!string.IsNullOrEmpty(ViewName))
            {
                // Use specific view name for regular requests
                viewResult.ViewName = ViewName;
            }
        }

        base.OnActionExecuted(context);
    }

    private static bool IsAjaxRequest(HttpRequest request)
    {
        return request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
               request.Headers.Accept.Any(a => a?.Contains("application/json") == true);
    }
}