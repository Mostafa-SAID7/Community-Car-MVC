using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;

public class ErrorReportVM
{
    public Guid Id { get; set; }
    public string TicketId { get; set; } = string.Empty;
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty; // Low, Medium, High, Critical
    public string Status { get; set; } = string.Empty; // Open, InProgress, Resolved, Closed
    public string? ReporterEmail { get; set; }
    public string? ReporterName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? AssignedTo { get; set; }
    public string? Resolution { get; set; }
    public List<string> Attachments { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}