namespace CommunityCar.Application.Features.Account.ViewModels.Media;

public class TagUsageVM
{
    public string Tag { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public DateTime LastUsed { get; set; }
    public string Color { get; set; } = string.Empty;
}