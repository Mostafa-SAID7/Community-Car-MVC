namespace CommunityCar.Application.Features.Dashboard.Performance.ViewModels;

public class SlowQueryVM
{
    public Guid Id { get; set; }
    public string Query { get; set; } = string.Empty;
    public double ExecutionTime { get; set; }
    public DateTime ExecutedAt { get; set; }
    public string Database { get; set; } = string.Empty;
    public string Table { get; set; } = string.Empty;
    public int RowsAffected { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? ApplicationName { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}