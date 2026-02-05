namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Media.SoftDelete.ViewModels;

/// <summary>
/// Response model for soft delete operations
/// </summary>
public class SoftDeleteResponseVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? Id { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}





