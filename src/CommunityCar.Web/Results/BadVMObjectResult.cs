using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CommunityCar.Web.Results;

/// <summary>
/// Custom action result for bad request with model state errors
/// </summary>
public class BadVMObjectResult : ObjectResult
{
    public BadVMObjectResult(ModelStateDictionary modelState) : base(new ValidationProblemDetails(modelState))
    {
        StatusCode = StatusCodes.Status400BadRequest;
    }

    public BadVMObjectResult(object error) : base(error)
    {
        StatusCode = StatusCodes.Status400BadRequest;
    }
}