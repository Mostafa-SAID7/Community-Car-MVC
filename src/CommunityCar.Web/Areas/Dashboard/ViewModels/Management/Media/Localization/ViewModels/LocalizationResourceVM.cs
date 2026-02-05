namespace CommunityCar.Application.Features.Dashboard.Management.Media.Localization.ViewModels;

public class LocalizationResourceVM
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Culture { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ResourceGroup { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public bool IsActive { get; set; }
}