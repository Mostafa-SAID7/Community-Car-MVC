using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Domain.Entities.Profile;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Profile;

public class UserProfileRepository : BaseRepository<UserProfile>, IUserProfileRepository
{
    public UserProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UserProfile?> GetByUserIdAsync(Guid userId)
    {
        return await DbSet
            .FirstOrDefaultAsync(up => up.UserId == userId);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId)
    {
        return await DbSet
            .AnyAsync(up => up.UserId == userId);
    }
}
