namespace CommunityCar.Web.Areas.Identity.ViewModels.Gamification;

public class PointTransactionVM
{
    public int Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
