using CommunityCar.AI.Models;

namespace CommunityCar.AI.Services;

public interface IGeminiChatService
{
    Task<SimpleChatResponse> GenerateChatResponseAsync(string message, string? context = null);
    Task<SimpleChatResponse> GenerateContextualResponseAsync(string message, List<ChatMessage> conversationHistory);
    Task<string> GenerateResponseAsync(string message, string? context = null);
    Task<bool> IsServiceAvailableAsync();
    Task<string> SummarizeConversationAsync(List<ChatMessage> messages);
    Task<List<string>> GenerateSuggestionsAsync(string context);
}

public interface IHuggingFaceChatService
{
    Task<SimpleChatResponse> GenerateChatResponseAsync(string message, string? context = null);
    Task<SimpleChatResponse> GenerateContextualResponseAsync(string message, List<ChatMessage> conversationHistory);
    Task<string> GenerateResponseAsync(string message, string? context = null);
    Task<bool> IsServiceAvailableAsync();
    Task<string> SummarizeConversationAsync(List<ChatMessage> messages);
    Task<List<string>> GenerateSuggestionsAsync(string context);
}