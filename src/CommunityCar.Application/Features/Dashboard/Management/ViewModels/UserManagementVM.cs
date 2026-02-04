namespace CommunityCar.Application.Features.Dashboard.Management.ViewModels;

public class UserManagementVM
{
    public List<UserSummaryVM> Users { get; set; } = new();
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int BannedUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public string FilterStatus { get; set; } = "All";
    public string SortBy { get; set; } = "CreatedAt";
    public string SortDirection { get; set; } = "Desc";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages { get; set; }
}

public class UserSummaryVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsBanned { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public int PostsCount { get; set; }
    public int CommentsCount { get; set; }
    public string Role { get; set; } = string.Empty;
}