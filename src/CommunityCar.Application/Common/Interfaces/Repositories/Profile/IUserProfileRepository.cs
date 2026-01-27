using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Profile;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Profile;

public interface IUserProfileRepository : IBaseRepository<UserProfile>
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId);
    Task<bool> ExistsByUserIdAsync(Guid userId);
}


