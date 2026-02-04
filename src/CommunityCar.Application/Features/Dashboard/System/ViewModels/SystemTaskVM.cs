using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.System.ViewModels;

public class SystemTaskVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Pending, Running, Completed, Failed
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public TimeSpan? Duration { get; set; }
    public double Progress { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Result { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}