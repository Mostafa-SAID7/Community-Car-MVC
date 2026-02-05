namespace CommunityCar.Web.Areas.Identity.ViewModels.Activity;

public class WhoViewedMyProfileVM
{
    public Guid UserId { get; set; }
    public int Views { get; set; }
    public int TotalCount { get; set; }
    public IEnumerable<ProfileViewerVM> Viewers { get; set; } = new List<ProfileViewerVM>();
    public int TotalViewers { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public ProfileViewStatsVM Stats { get; set; } = new();
}
