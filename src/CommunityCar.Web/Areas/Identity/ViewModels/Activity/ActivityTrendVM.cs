namespace CommunityCar.Web.Areas.Identity.ViewModels.Activity;

public class ActivityTrendVM
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string DateLabel { get; set; } = string.Empty;
}
