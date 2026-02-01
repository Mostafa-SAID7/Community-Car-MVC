namespace CommunityCar.Application.Features.Account.ViewModels.Authorization;

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = "Custom";
    public int Priority { get; set; }
    public List<string> Permissions { get; set; } = new();
}