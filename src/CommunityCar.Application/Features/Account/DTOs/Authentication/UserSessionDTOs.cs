namespace CommunityCar.Application.Features.Account.DTOs.Authentication;

#region User Session DTOs

public class UserSessionDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsSuspicious { get; set; }
}

public class CreateSessionRequest
{
    public Guid UserId { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? Location { get; set; }
}

public class UpdateSessionActivityRequest
{
    public string SessionId { get; set; } = string.Empty;
    public DateTime LastActivityAt { get; set; }
}

public class EndSessionRequest
{
    public string SessionId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}

#endregion