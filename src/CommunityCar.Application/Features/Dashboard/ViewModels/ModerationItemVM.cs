namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class ModerationItemVM
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ReportCount { get; set; }
    public List<string> ReportReasons { get; set; } = new();
}