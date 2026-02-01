using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc.Localization;

namespace CommunityCar.Web.Controllers.Test
{
    [Route("test/localization")]
    public class LocalizationTestController : Controller
    {
        private readonly IStringLocalizerFactory _localizerFactory;
        private readonly IViewLocalizer _viewLocalizer;

        public LocalizationTestController(IStringLocalizerFactory localizerFactory, IViewLocalizer viewLocalizer)
        {
            _localizerFactory = localizerFactory;
            _viewLocalizer = viewLocalizer;
        }

        [HttpGet("check")]
        public IActionResult Check()
        {
            // Simulate looking up a resource for the Feed Index view
            var typeName = "CommunityCar.Web.Views.Community.Feed.Index";
            var assemblyName = "CommunityCar.Web";
            
            // Create a localizer manually to see if it works
            var localizer = _localizerFactory.Create("Views.Community.Feed.Index", assemblyName);
            var key = "PageTitle";
            var value = localizer[key];

            return Json(new
            {
                SearchedLocation = "Views.Community.Feed.Index",
                Key = key,
                Found = !value.ResourceNotFound,
                Value = value.Value,
                CurrentCulture = System.Globalization.CultureInfo.CurrentCulture.Name,
                CurrentUICulture = System.Globalization.CultureInfo.CurrentUICulture.Name
            });
        }
        
        [HttpGet("view-check")]
        public IActionResult ViewCheck()
        {
             // return the view that we will create to test IViewLocalizer directly
             return View("~/Views/Community/Feed/Index.cshtml", new CommunityCar.Application.Features.Community.Feed.DTOs.FeedResponse()); 
        }
    }
}
