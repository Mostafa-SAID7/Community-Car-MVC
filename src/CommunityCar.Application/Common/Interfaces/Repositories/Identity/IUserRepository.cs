using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Auth;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Identity;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUserNameAsync(string userName);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);
    Task<bool> IsUserNameUniqueAsync(string userName, Guid? excludeUserId = null);
}