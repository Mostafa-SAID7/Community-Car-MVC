namespace CommunityCar.AI.Models;

/// <summary>
/// Simple chat response model for basic AI services
/// </summary>
public class SimpleChatResponse
{
    public string Message { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public DateTime Timestamp { get; set; }
    public string Source { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}