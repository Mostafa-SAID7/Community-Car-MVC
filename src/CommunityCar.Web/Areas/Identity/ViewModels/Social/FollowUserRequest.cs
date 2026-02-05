namespace CommunityCar.Web.Areas.Identity.ViewModels.Social;

public class FollowUserRequest
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
}
