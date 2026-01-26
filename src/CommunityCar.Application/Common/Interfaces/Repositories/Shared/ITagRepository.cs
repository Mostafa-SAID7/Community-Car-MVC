using CommunityCar.Domain.Entities.Shared;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(Guid id);
    Task<Tag?> GetBySlugAsync(string slug);
    Task<Tag?> GetByNameAsync(string name);
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<IEnumerable<Tag>> GetPopularTagsAsync(int count = 20);
    Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm, int count = 10);
    Task<Tag> AddAsync(Tag tag);
    Task<Tag> UpdateAsync(Tag tag);
    Task DeleteAsync(Tag tag);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
}