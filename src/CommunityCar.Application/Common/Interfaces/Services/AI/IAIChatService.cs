namespace CommunityCar.Application.Common.Interfaces.Services.AI;

public interface IAIChatService
{
    Task<string> GenerateResponseAsync(string message, string? conversationId = null);
    Task<string> StartConversationAsync();
    Task EndConversationAsync(string conversationId);
}