namespace CommunityCar.Web.Areas.Identity.ViewModels.Social;

public class AddInterestRequest
{
    public Guid UserId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string InterestType { get; set; } = string.Empty;
    public string InterestValue { get; set; } = string.Empty;
    public double InitialScore { get; set; } = 1.0;
}
