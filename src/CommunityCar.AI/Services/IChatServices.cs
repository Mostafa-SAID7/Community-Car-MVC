using CommunityCar.AI.Models;

namespace CommunityCar.AI.Services;

public interface IGeminiChatService
{
    Task<ChatResponse> GenerateChatResponseAsync(string message, string? context = null);
    Task<ChatResponse> GenerateContextualResponseAsync(string message, List<ChatMessage> conversationHistory);
    Task<bool> IsServiceAvailableAsync();
    Task<string> SummarizeConversationAsync(List<ChatMessage> messages);
    Task<List<string>> GenerateSuggestionsAsync(string context);
}

public interface IHuggingFaceChatService
{
    Task<ChatResponse> GenerateChatResponseAsync(string message, string? context = null);
    Task<ChatResponse> GenerateContextualResponseAsync(string message, List<ChatMessage> conversationHistory);
    Task<bool> IsServiceAvailableAsync();
    Task<string> SummarizeConversationAsync(List<ChatMessage> messages);
    Task<List<string>> GenerateSuggestionsAsync(string context);
}