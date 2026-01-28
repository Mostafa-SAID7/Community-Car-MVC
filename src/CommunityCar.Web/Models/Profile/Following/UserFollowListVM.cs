using CommunityCar.Application.Common.Models;

namespace CommunityCar.Web.Models.Profile.Following;

/// <summary>
/// View model for displaying following/followers lists
/// </summary>
public class UserFollowListVM
{
    public Guid ProfileUserId { get; set; }
    public string ProfileUserName { get; set; } = string.Empty;
    public string ListType { get; set; } = string.Empty; // "following" or "followers"
    public List<FollowingVM> Users { get; set; } = new();
    public PaginationInfo Pagination { get; set; } = new();
    public int TotalCount { get; set; }
    public bool CanViewList { get; set; } = true;
    public bool IsOwnProfile { get; set; }
}