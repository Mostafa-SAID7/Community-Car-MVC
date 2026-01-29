namespace CommunityCar.Application.Features.Account.DTOs.Social;

#region User Interest DTOs

public class UserInterestDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid InterestId { get; set; }
    public string InterestName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AddInterestRequest
{
    public Guid UserId { get; set; }
    public Guid InterestId { get; set; }
    public int Priority { get; set; } = 0;
}

public class UpdateInterestRequest
{
    public Guid UserId { get; set; }
    public Guid InterestId { get; set; }
    public int? Priority { get; set; }
    public string? Category { get; set; }
}

public class RemoveInterestRequest
{
    public Guid UserId { get; set; }
    public Guid InterestId { get; set; }
}

#endregion