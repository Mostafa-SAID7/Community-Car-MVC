using CommunityCar.Domain.Entities.Account.Core;
using System.Linq.Expressions;

namespace CommunityCar.Domain.Specifications.Account;

/// <summary>
/// Specifications for User entity queries
/// </summary>
public static class UserSpecifications
{
    /// <summary>
    /// Specification for active users
    /// </summary>
    public class ActiveUsersSpec : BaseSpecification<User>
    {
        public ActiveUsersSpec() : base(u => u.IsActive && !u.IsDeleted)
        {
        }
    }

    /// <summary>
    /// Specification for users by email
    /// </summary>
    public class UserByEmailSpec : BaseSpecification<User>
    {
        public UserByEmailSpec(string email) : base(u => u.Email == email && !u.IsDeleted)
        {
        }
    }

    /// <summary>
    /// Specification for users by username
    /// </summary>
    public class UserByUsernameSpec : BaseSpecification<User>
    {
        public UserByUsernameSpec(string username) : base(u => u.UserName == username && !u.IsDeleted)
        {
        }
    }

    /// <summary>
    /// Specification for users with public profiles
    /// </summary>
    public class PublicProfileUsersSpec : BaseSpecification<User>
    {
        public PublicProfileUsersSpec() : base(u => u.PrivacySettings.IsPublic && u.IsActive && !u.IsDeleted)
        {
            AddOrderBy(u => u.Profile.FullName);
        }
    }

    /// <summary>
    /// Specification for users with two-factor authentication enabled
    /// </summary>
    public class TwoFactorEnabledUsersSpec : BaseSpecification<User>
    {
        public TwoFactorEnabledUsersSpec() : base(u => u.TwoFactorSettings.TwoFactorEnabled && !u.IsDeleted)
        {
        }
    }

    /// <summary>
    /// Specification for users by role with minimum points
    /// </summary>
    public class UsersByRoleAndPointsSpec : BaseSpecification<User>
    {
        public UsersByRoleAndPointsSpec(int minPoints) : base(u => u.TotalPoints >= minPoints && u.IsActive && !u.IsDeleted)
        {
            AddOrderByDescending(u => u.TotalPoints);
        }
    }

    /// <summary>
    /// Specification for recently registered users
    /// </summary>
    public class RecentlyRegisteredUsersSpec : BaseSpecification<User>
    {
        public RecentlyRegisteredUsersSpec(int daysBack = 30) 
            : base(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-daysBack) && !u.IsDeleted)
        {
            AddOrderByDescending(u => u.CreatedAt);
        }
    }

    /// <summary>
    /// Specification for users with profile pictures
    /// </summary>
    public class UsersWithProfilePicturesSpec : BaseSpecification<User>
    {
        public UsersWithProfilePicturesSpec() 
            : base(u => !string.IsNullOrEmpty(u.Profile.ProfilePictureUrl) && u.IsActive && !u.IsDeleted)
        {
        }
    }

    /// <summary>
    /// Specification for users by location
    /// </summary>
    public class UsersByLocationSpec : BaseSpecification<User>
    {
        public UsersByLocationSpec(string city, string? country = null) 
            : base(u => u.Profile.City == city && 
                       (country == null || u.Profile.Country == country) && 
                       u.IsActive && !u.IsDeleted)
        {
            AddOrderBy(u => u.Profile.FullName);
        }
    }

    /// <summary>
    /// Specification for searching users by name
    /// </summary>
    public class UserSearchSpec : BaseSpecification<User>
    {
        public UserSearchSpec(string searchTerm) 
            : base(u => (u.Profile.FullName.Contains(searchTerm) || 
                        u.Profile.FirstName.Contains(searchTerm) || 
                        u.Profile.LastName.Contains(searchTerm) ||
                        u.UserName.Contains(searchTerm)) && 
                       u.IsActive && !u.IsDeleted)
        {
            AddOrderBy(u => u.Profile.FullName);
        }
    }
}