namespace CommunityCar.Application.Features.Account.DTOs.Social;

#region User Profile View DTOs

public class UserProfileViewDTO
{
    public Guid Id { get; set; }
    public Guid ViewerId { get; set; }
    public Guid ProfileUserId { get; set; }
    public string ViewerName { get; set; } = string.Empty;
    public string? ViewerProfilePicture { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public string? Location { get; set; }
    public DateTime ViewedAt { get; set; }
    public bool IsAnonymous { get; set; }
}

public class RecordProfileViewRequest
{
    public Guid ViewerId { get; set; }
    public Guid ProfileUserId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Location { get; set; }
}

public class ProfileViewsRequest
{
    public Guid ProfileUserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? IsAnonymous { get; set; }
}

#endregion