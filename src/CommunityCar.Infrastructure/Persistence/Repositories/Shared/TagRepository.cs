using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Shared;

public class TagRepository : BaseRepository<Tag>, ITagRepository
{
    public TagRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Tag?> GetBySlugAsync(string slug)
    {
        return await _context.Set<Tag>()
            .FirstOrDefaultAsync(t => t.Slug == slug);
    }

    public async Task<Tag?> GetByNameAsync(string name)
    {
        return await _context.Set<Tag>()
            .FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<IEnumerable<Tag>> GetPopularTagsAsync(int count = 20)
    {
        return await _context.Set<Tag>()
            .OrderByDescending(t => t.UsageCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm, int count = 10)
    {
        return await _context.Set<Tag>()
            .Where(t => t.Name.Contains(searchTerm))
            .OrderByDescending(t => t.UsageCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
    {
        var query = _context.Set<Tag>().Where(t => t.Name == name);
        
        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId.Value);
            
        return await query.AnyAsync();
    }

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null)
    {
        var query = _context.Set<Tag>().Where(t => t.Slug == slug);
        
        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId.Value);
            
        return await query.AnyAsync();
    }
}