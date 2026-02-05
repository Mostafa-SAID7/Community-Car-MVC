namespace CommunityCar.Application.Features.Dashboard.Management.developer.Performance.ViewModels;

/// <summary>
/// ViewModel for slow database queries
/// </summary>
public class SlowQueryVM
{
    public Guid Id { get; set; }
    public string Query { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public double ExecutionTime { get; set; }
    public DateTime ExecutedAt { get; set; }
    public int RowsAffected { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string Source { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}