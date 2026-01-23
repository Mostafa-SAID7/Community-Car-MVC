using CommunityCar.Application.Common.Interfaces.Services.Community;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Community.Feed;

[ApiController]
[Route("api/feed")]
public class FeedApiController : ControllerBase
{
    private readonly IStoriesService _storiesService;

    public FeedApiController(IStoriesService storiesService)
    {
        _storiesService = storiesService;
    }

    [HttpGet("stories")]
    public async Task<IActionResult> GetStories()
    {
        var stories = await _storiesService.GetActiveStoriesAsync();
        return Ok(stories);
    }
}
