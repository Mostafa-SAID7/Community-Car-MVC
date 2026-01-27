namespace CommunityCar.Application.Features.Identity.ViewModels;

/// <summary>
/// View model for role information
/// </summary>
public class RoleVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string ConcurrencyStamp { get; set; } = string.Empty;
}


