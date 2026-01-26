using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Domain.Entities.Shared;
using Microsoft.AspNetCore.Authorization;

namespace CommunityCar.Web.Controllers.Shared.Categories;

[Route("api/shared/categories")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUserService;

    public CategoriesController(
        ICategoryRepository categoryRepository,
        ICurrentUserService currentUserService)
    {
        _categoryRepository = categoryRepository;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("root")]
    public async Task<IActionResult> GetRootCategories()
    {
        try
        {
            var categories = await _categoryRepository.GetRootCategoriesAsync();
            return Ok(categories);
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
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
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
            var category = await _categoryRepository.GetBySlugAsync(slug);
            if (category == null)
                return NotFound();

            return Ok(category);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{parentId}/children")]
    public async Task<IActionResult> GetChildren(Guid parentId)
    {
        try
        {
            var categories = await _categoryRepository.GetByParentIdAsync(parentId);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        try
        {
            if (await _categoryRepository.SlugExistsAsync(request.Slug))
                return BadRequest(new { success = false, message = "Category slug already exists" });

            var category = new Category(request.Name, request.Slug, request.Description, request.ParentCategoryId);
            await _categoryRepository.AddAsync(category);
            
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            if (await _categoryRepository.SlugExistsAsync(request.Slug, id))
                return BadRequest(new { success = false, message = "Category slug already exists" });

            // Note: Category entity would need update methods
            await _categoryRepository.UpdateAsync(category);
            return Ok(category);
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
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            await _categoryRepository.DeleteAsync(category);
            return Ok(new { success = true, message = "Category deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
}

public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
}