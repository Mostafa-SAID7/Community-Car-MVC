namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Media.SoftDelete.ViewModels;

/// <summary>
/// Request model for restore operations
/// </summary>
public class RestoreRequestVM
{
    public Guid Id { get; set; }
    public string? Reason { get; set; }
    public bool NotifyUser { get; set; } = false;
}





