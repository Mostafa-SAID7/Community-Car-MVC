namespace CommunityCar.AI.Models;

/// <summary>
/// User segment classification
/// </summary>
public class UserSegment
{
    public string SegmentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Characteristics { get; set; } = new();
    public double Score { get; set; }
}