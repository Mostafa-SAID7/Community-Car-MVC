using CommunityCar.Web.Models.Error;
using CommunityCar.Web.Middleware;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace CommunityCar.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public IActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View("404");
        }

        [Route("Error/500")]
        public IActionResult ServerError()
        {
            Response.StatusCode = 500;
            return View("500");
        }

        [Route("Error/Maintenance")]
        public IActionResult Maintenance()
        {
            // Set status code to 503 (Service Unavailable) for SEO/Bots
            Response.StatusCode = 503;
            return View("Maintenance");
        }

        [Route("Error/Details/{errorId}")]
        public IActionResult Details(string errorId, string? data = null)
        {
            var model = new ErrorDetailsViewModel
            {
                ErrorId = errorId,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            // Parse error data from query string if provided
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    var decodedData = Uri.UnescapeDataString(data);
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(decodedData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (errorResponse != null)
                    {
                        model.Message = errorResponse.Message;
                        model.StatusCode = errorResponse.StatusCode;
                        model.Details = errorResponse.Details;
                        model.StackTrace = errorResponse.StackTrace;
                        model.Path = errorResponse.Path;
                        model.Method = errorResponse.Method;
                        model.Timestamp = errorResponse.Timestamp;
                    }
                }
                catch (Exception ex)
                {
                    // Log the parsing error but continue to show basic error page
                    Console.WriteLine($"Failed to parse error data: {ex.Message}");
                }
            }

            return View("Details", model);
        }

        [Route("Error/{statusCode}")]
        public IActionResult HandleErrorCode(int statusCode)
        {
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return statusCode switch
            {
                404 => View("404"),
                500 => View("500"),
                503 => View("Maintenance"),
                _ => View("GenericError", model)
            };
        }
    }
}



