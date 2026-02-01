namespace CommunityCar.Application.Features.Account.ViewModels.Authorization;

public class UserRoleSummaryVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public DateTime AssignedAt { get; set; }
}