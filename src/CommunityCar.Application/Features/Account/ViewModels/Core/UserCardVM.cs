namespace CommunityCar.Application.Features.Account.ViewModels.Core;

public class UserCardVM
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool IsFollowing { get; set; }
    public int FollowersCount { get; set; }
    public int PostsCount { get; set; }
}