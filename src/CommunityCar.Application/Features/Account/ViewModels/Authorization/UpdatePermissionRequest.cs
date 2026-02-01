namespace CommunityCar.Application.Features.Account.ViewModels.Authorization;

public class UpdatePermissionRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
}