namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Content.ViewModels;

public class ModerateContentVM
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Approve, Reject, Delete
    public string Reason { get; set; } = string.Empty;
    public string ModeratorNotes { get; set; } = string.Empty;
    public bool NotifyAuthor { get; set; } = true;
    public bool BanUser { get; set; } = false;
    public int BanDurationDays { get; set; } = 0;
}




