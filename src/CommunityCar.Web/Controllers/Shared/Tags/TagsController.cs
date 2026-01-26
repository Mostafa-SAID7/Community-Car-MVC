using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Domain.Entities.Shared;
using Microsoft.AspNetCore.Authorization;

namespace CommunityCar.Web.Controllers.Shared.Tags;

[Route("api/[controller]")]
[ApiController]
public class TagsController : ControllerBase
{
    private readonly ITagRepository _tagRepository;
    private readonly ICurrentUserService _currentUserService;

    public TagsController(
        ITagRepository tagRepository,
        ICurrentUserService currentUserService)
    {
        _tagRepository = tagRepository;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var tags = await _tagRepository.GetAllAsync();
            return Ok(tags);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("popular")]
    public async Task<IActionResult> GetPopular([FromQuery] int count = 20)
    {
        try
        {
            var tags = await _tagRepository.GetPopularTagsAsync(count);
            return Ok(tags);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term, [FromQuery] int count = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest(new { success = false, message = "Search term is required" });

            var tags = await _tagRepository.SearchTagsAsync(term, count);
            return Ok(tags);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
                return NotFound();

            return Ok(tag);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        try
        {
            var tag = await _tagRepository.GetBySlugAsync(slug);
            if (tag == null)
                return NotFound();

            return Ok(tag);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        try
        {
            var tag = await _tagRepository.GetByNameAsync(name);
            if (tag == null)
                return NotFound();

            return Ok(tag);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest request)
    {
        try
        {
            if (await _tagRepository.NameExistsAsync(request.Name))
                return BadRequest(new { success = false, message = "Tag name already exists" });

            if (await _tagRepository.SlugExistsAsync(request.Slug))
                return BadRequest(new { success = false, message = "Tag slug already exists" });

            var tag = new Tag(request.Name, request.Slug);
            var createdTag = await _tagRepository.AddAsync(tag);
            
            return CreatedAtAction(nameof(GetById), new { id = createdTag.Id }, createdTag);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTagRequest request)
    {
        try
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
                return NotFound();

            if (await _tagRepository.NameExistsAsync(request.Name, id))
                return BadRequest(new { success = false, message = "Tag name already exists" });

            if (await _tagRepository.SlugExistsAsync(request.Slug, id))
                return BadRequest(new { success = false, message = "Tag slug already exists" });

            // Note: Tag entity would need update methods
            var updatedTag = await _tagRepository.UpdateAsync(tag);
            return Ok(updatedTag);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
                return NotFound();

            await _tagRepository.DeleteAsync(tag);
            return Ok(new { success = true, message = "Tag deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{id}/increment-usage")]
    public async Task<IActionResult> IncrementUsage(Guid id)
    {
        try
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
                return NotFound();

            tag.IncrementUsage();
            await _tagRepository.UpdateAsync(tag);
            
            return Ok(new { success = true, usageCount = tag.UsageCount });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class CreateTagRequest
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

public class UpdateTagRequest
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}