namespace CommunityCar.Application.Features.Community.SoftDelete.ViewModels;

/// <summary>
/// Response model for restore operations
/// </summary>
public class RestoreResponseVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? Id { get; set; }
    public DateTime? RestoredAt { get; set; }
    public string? RestoredBy { get; set; }
}