namespace CommunityCar.AI.Models;

/// <summary>
/// Chat context data for intelligent responses
/// </summary>
public class ChatContextData
{
    public string UserId { get; set; } = string.Empty;
    public string ConversationId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Intent { get; set; } = string.Empty;
    public List<string> Entities { get; set; } = new();
    public Dictionary<string, object> Context { get; set; } = new();
    public DateTime Timestamp { get; set; }
    public string Language { get; set; } = "en";
}