using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Shared;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetBySlugAsync(string slug)
    {
        return await Context.Set<Category>()
            .FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public async Task<IEnumerable<Category>> GetByParentIdAsync(Guid? parentId)
    {
        return await Context.Set<Category>()
            .Where(c => c.ParentCategoryId == parentId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
    {
        return await Context.Set<Category>()
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null)
    {
        var query = Context.Set<Category>().Where(c => c.Slug == slug);
        
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
            
        return await query.AnyAsync();
    }
}