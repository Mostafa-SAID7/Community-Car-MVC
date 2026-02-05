namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Media.SoftDelete.ViewModels;

/// <summary>
/// Request model for soft delete operations
/// </summary>
public class SoftDeleteRequestVM
{
    public Guid Id { get; set; }
    public string? Reason { get; set; }
    public bool NotifyUser { get; set; } = false;
}





