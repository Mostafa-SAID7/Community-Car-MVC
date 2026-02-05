namespace CommunityCar.Web.Areas.Identity.ViewModels.Social;

public class FollowingListVM
{
    public Guid UserId { get; set; }
    public string ListType { get; set; } = string.Empty; // "following" or "followers"
    public List<NetworkUserVM> Users { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasMore { get; set; }
    public string? SearchTerm { get; set; }
}
