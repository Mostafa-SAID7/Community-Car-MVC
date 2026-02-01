namespace CommunityCar.Application.Features.Error.ViewModels;

public class ErrorDetailsViewModel
{
    public string? ErrorId { get; set; }
    public string? RequestId { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Path { get; set; }
    public string? Method { get; set; }
    public string? Details { get; set; }
    public string? StackTrace { get; set; }
}