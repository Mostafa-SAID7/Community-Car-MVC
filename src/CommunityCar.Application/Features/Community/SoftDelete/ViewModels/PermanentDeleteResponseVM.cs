namespace CommunityCar.Application.Features.Community.SoftDelete.ViewModels;

/// <summary>
/// Response model for permanent delete operations
/// </summary>
public class PermanentDeleteResponseVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? Id { get; set; }
    public DateTime? PermanentlyDeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}