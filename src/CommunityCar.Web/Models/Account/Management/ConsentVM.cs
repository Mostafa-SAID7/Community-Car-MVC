namespace CommunityCar.Web.Models.Account.Management;

public class ConsentVM
{
    public string Type { get; set; } = string.Empty;
    public bool IsAccepted { get; set; }
    public DateTime AcceptedAt { get; set; }
    public string Version { get; set; } = "1.0";
    public string Description { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
}