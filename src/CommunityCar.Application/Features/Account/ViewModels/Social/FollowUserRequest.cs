namespace CommunityCar.Application.Features.Account.ViewModels.Social;

public class FollowUserRequest
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
}