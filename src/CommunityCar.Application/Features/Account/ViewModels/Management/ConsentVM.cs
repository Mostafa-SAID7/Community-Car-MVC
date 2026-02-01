namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class ConsentVM
{
    public string Type { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public bool IsAccepted { get; set; }
    public DateTime AcceptedAt { get; set; }
    public string IpAddress { get; set; } = string.Empty;
}