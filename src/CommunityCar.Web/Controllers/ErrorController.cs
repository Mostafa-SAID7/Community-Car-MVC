using CommunityCar.Web.Models;
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
        public IActionResult Details(string errorId)
        {
            // In a real application, you would fetch error details from database
            // For now, we'll create a mock error details view
            var model = new ErrorDetailsViewModel
            {
                ErrorId = errorId,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

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
