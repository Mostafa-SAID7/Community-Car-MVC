namespace CommunityCar.Application.Features.Dashboard.SoftDelete.ViewModels;

/// <summary>
/// Request model for restore operations
/// </summary>
public class RestoreRequestVM
{
    public Guid Id { get; set; }
    public string? Reason { get; set; }
    public bool NotifyUser { get; set; } = false;
}
