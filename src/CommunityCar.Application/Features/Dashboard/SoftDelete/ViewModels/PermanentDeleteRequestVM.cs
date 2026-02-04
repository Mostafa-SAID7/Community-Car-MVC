namespace CommunityCar.Application.Features.Dashboard.SoftDelete.ViewModels;

/// <summary>
/// Request model for permanent delete operations
/// </summary>
public class PermanentDeleteRequestVM
{
    public Guid Id { get; set; }
    public string? Reason { get; set; }
    public bool ConfirmPermanentDelete { get; set; } = false;
}
