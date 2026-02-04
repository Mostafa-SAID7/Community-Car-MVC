namespace CommunityCar.Application.Features.Dashboard.Performance.ViewModels;

public class ProcessInfoVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double CpuUsage { get; set; }
    public long MemoryUsage { get; set; }
    public int ThreadCount { get; set; }
    public DateTime StartTime { get; set; }
}