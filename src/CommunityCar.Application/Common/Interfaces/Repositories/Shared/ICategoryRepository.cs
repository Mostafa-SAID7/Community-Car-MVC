using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Shared;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug);
    Task<IEnumerable<Category>> GetByParentIdAsync(Guid? parentId);
    Task<IEnumerable<Category>> GetRootCategoriesAsync();
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
}