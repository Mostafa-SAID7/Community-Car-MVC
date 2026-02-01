namespace CommunityCar.Application.Features.Account.ViewModels.Authorization;

public class CreatePermissionRequest
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Category { get; set; } = "Custom";
    public string? Description { get; set; }
    public bool IsSystemPermission { get; set; }
}