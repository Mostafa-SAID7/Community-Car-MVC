using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Features.Account.ViewModels.Social;

public class UserFollowListVM
{
    public Guid ProfileUserId { get; set; }
    public string ProfileUserName { get; set; } = string.Empty;
    public string ListType { get; set; } = string.Empty;
    public IEnumerable<FollowingVM> Users { get; set; } = new List<FollowingVM>();
    public int TotalCount { get; set; }
    public bool IsOwnProfile { get; set; }
    public PaginationInfo Pagination { get; set; } = new();
}