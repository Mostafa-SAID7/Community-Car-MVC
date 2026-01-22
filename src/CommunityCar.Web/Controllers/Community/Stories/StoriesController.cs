using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Stories.DTOs;
using CommunityCar.Application.Features.Stories.ViewModels;

namespace CommunityCar.Web.Controllers.Community.Stories;

[Route("stories")]
public class StoriesController : Controller
{
    private readonly IStoriesService _storiesService;

    public StoriesController(IStoriesService storiesService)
    {
        _storiesService = storiesService;
    }

    // Redirect to Feed page since Stories are now integrated there
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Feed");
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateStoryRequest request)
    {
        try
        {
            // TODO: Get current user ID from authentication
            request.AuthorId = Guid.NewGuid(); // Temporary - replace with actual user ID
            
            var story = await _storiesService.CreateAsync(request);
            return Json(new { success = true, story });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var story = await _storiesService.GetByIdAsync(id);
        if (story == null)
            return NotFound();

        return Json(story);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStoryRequest request)
    {
        try
        {
            var story = await _storiesService.UpdateAsync(id, request);
            return Json(new { success = true, story });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _storiesService.DeleteAsync(id);
        return Json(new { success = result });
    }

    [HttpPost("{id}/like")]
    public async Task<IActionResult> Like(Guid id)
    {
        try
        {
            // TODO: Get current user ID from authentication
            var userId = Guid.NewGuid(); // Temporary
            var result = await _storiesService.LikeAsync(id, userId);
            return Json(new { success = result });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{id}/view")]
    public async Task<IActionResult> IncrementView(Guid id)
    {
        var result = await _storiesService.IncrementViewCountAsync(id);
        return Json(new { success = result });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] StoriesSearchRequest request)
    {
        var result = await _storiesService.SearchStoriesAsync(request);
        return Json(result);
    }

    [HttpGet("tags")]
    public async Task<IActionResult> GetPopularTags([FromQuery] int count = 20)
    {
        var tags = await _storiesService.GetPopularTagsAsync(count);
        return Json(tags);
    }

    [HttpGet("car-makes")]
    public async Task<IActionResult> GetCarMakes()
    {
        var carMakes = await _storiesService.GetAvailableCarMakesAsync();
        return Json(carMakes);
    }
}
