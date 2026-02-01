using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CommunityCar.Web.Helpers;
using CommunityCar.Web.Results;

namespace CommunityCar.Web.Attributes;

/// <summary>
/// Validates model state and returns appropriate response for invalid models
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ValidateModelAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Whether to return JSON response for AJAX requests
    /// </summary>
    public bool ReturnJsonForAjax { get; set; } = true;

    /// <summary>
    /// Custom error message for validation failures
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// HTTP status code to return for validation failures
    /// </summary>
    public int StatusCode { get; set; } = 400;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var isAjaxVM = context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpVM" ||
                               context.HttpContext.Request.Headers.Accept.Any(a => a?.Contains("application/json") == true);

            if (ReturnJsonForAjax && isAjaxVM)
            {
                // Return JSON response for AJAX requests
                var errors = ValidationHelper.GetModelStateErrors(context.ModelState);
                context.Result = JsonVMHelper.ValidationError(errors);
            }
            else
            {
                // Return view with validation errors for regular requests
                var controller = context.Controller as Controller;
                if (controller != null)
                {
                    if (!string.IsNullOrEmpty(ErrorMessage))
                    {
                        controller.ModelState.AddModelError(string.Empty, ErrorMessage);
                    }
                    
                    // Return the current view with validation errors
                    context.Result = new ViewResult
                    {
                        ViewData = controller.ViewData,
                        TempData = controller.TempData
                    };
                }
                else
                {
                    context.Result = new BadVMObjectResult(context.ModelState);
                }
            }
        }

        base.OnActionExecuting(context);
    }
}

/// <summary>
/// Validates specific model properties
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ValidatePropertiesAttribute : ActionFilterAttribute
{
    private readonly string[] _propertyNames;

    public ValidatePropertiesAttribute(params string[] propertyNames)
    {
        _propertyNames = propertyNames ?? throw new ArgumentNullException(nameof(propertyNames));
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var hasErrors = false;

        foreach (var propertyName in _propertyNames)
        {
            if (context.ModelState.ContainsKey(propertyName) && 
                context.ModelState[propertyName]?.Errors.Count > 0)
            {
                hasErrors = true;
                break;
            }
        }

        if (hasErrors)
        {
            var isAjaxVM = context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpVM";
            
            if (isAjaxVM)
            {
                var errors = ValidationHelper.GetModelStateErrors(context.ModelState)
                    .Where(kvp => _propertyNames.Contains(kvp.Key))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                context.Result = JsonVMHelper.ValidationError(errors);
            }
            else
            {
                context.Result = new BadVMObjectResult(context.ModelState);
            }
        }

        base.OnActionExecuting(context);
    }
}