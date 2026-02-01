namespace CommunityCar.Application.Features.Account.ViewModels.Authorization;

public class AssignRoleRequest
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; } = string.Empty;
}