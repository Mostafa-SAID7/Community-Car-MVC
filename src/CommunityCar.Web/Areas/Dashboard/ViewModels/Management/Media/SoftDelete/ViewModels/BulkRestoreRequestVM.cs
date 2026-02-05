namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Media.SoftDelete.ViewModels;

/// <summary>
/// Request model for bulk restore operations
/// </summary>
public class BulkRestoreRequestVM
{
    public IEnumerable<Guid> Ids { get; set; } = new List<Guid>();
    public string? Reason { get; set; }
    public bool NotifyUsers { get; set; } = false;
}





