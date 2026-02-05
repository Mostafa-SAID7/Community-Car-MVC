namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.developer.ErrorReporting.ViewModels;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime? Timestamp { get; set; }
}




