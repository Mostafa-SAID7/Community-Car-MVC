namespace CommunityCar.Application.Features.Dashboard.Management.Media.SoftDelete.ViewModels;

/// <summary>
/// Request model for bulk soft delete operations
/// </summary>
public class BulkSoftDeleteRequestVM
{
    public IEnumerable<Guid> Ids { get; set; } = new List<Guid>();
    public string? Reason { get; set; }
    public bool NotifyUsers { get; set; } = false;
}
