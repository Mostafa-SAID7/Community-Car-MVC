namespace CommunityCar.AI.Models;

/// <summary>
/// Car-specific context analysis
/// </summary>
public class CarRelatedContext
{
    public List<string> CarBrands { get; set; } = new();
    public List<string> CarModels { get; set; } = new();
    public List<string> CarFeatures { get; set; } = new();
    public List<string> Issues { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public float RelevanceScore { get; set; }
}