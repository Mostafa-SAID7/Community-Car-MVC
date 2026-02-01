namespace CommunityCar.Application.Features.ErrorReporting.ViewModels;

public class ErrorReportVM
{
    public string ErrorId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string StepsToReproduce { get; set; } = string.Empty;
    public string ExpectedBehavior { get; set; } = string.Empty;
    public string ActualBehavior { get; set; } = string.Empty;
    public string BrowserInfo { get; set; } = string.Empty;
    public string DeviceInfo { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public List<string> AttachmentUrls { get; set; } = new();
    
    // Response properties
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TicketId { get; set; } = string.Empty;
}

public class ErrorReportResponseVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TicketId { get; set; } = string.Empty;
}