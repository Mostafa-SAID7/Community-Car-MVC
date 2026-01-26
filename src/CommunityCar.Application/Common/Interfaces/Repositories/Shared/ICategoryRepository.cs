using CommunityCar.Domain.Entities.Shared;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id);
    Task<Category?> GetBySlugAsync(string slug);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<IEnumerable<Category>> GetByParentIdAsync(Guid? parentId);
    Task<IEnumerable<Category>> GetRootCategoriesAsync();
    Task<Category> AddAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task DeleteAsync(Category category);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
}