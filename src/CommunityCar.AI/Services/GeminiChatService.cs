using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityCar.Application.Common.Interfaces.Services.AI;
using CommunityCar.AI.Configuration;
using CommunityCar.AI.Models;
using Microsoft.Extensions.Options;

namespace CommunityCar.AI.Services;

public class GeminiChatService : IAIChatService, IGeminiChatService
{
    private readonly HttpClient _httpClient;
    private readonly AISettings _settings;

    public GeminiChatService(HttpClient httpClient, IOptions<AISettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<string> GenerateResponseAsync(string message, string? conversationId = null)
    {
        var apiKey = _settings.Gemini.ApiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            return "Error: Gemini API Key is missing.";
        }

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = message }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return $"Error communicating with Gemini: {response.StatusCode} - {errorBody}";
            }

            var responseString = await response.Content.ReadAsStringAsync();
            
            // Basic parsing of Gemini response structure
            using var doc = JsonDocument.Parse(responseString);
            var root = doc.RootElement;
            
            if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {
                var firstCandidate = candidates[0];
                if (firstCandidate.TryGetProperty("content", out var candidateContent) && 
                    candidateContent.TryGetProperty("parts", out var parts) && 
                    parts.GetArrayLength() > 0)
                {
                    return parts[0].GetProperty("text").GetString() ?? "No response text found.";
                }
            }
            
            return "No valid response from Gemini.";

        }
        catch (Exception ex)
        {
            return $"Exception communicating with Gemini: {ex.Message}";
        }
    }

    public Task<string> StartConversationAsync()
    {
        // Generate a simple conversation ID for Gemini
        var conversationId = Guid.NewGuid().ToString();
        return Task.FromResult(conversationId);
    }

    public Task EndConversationAsync(string conversationId)
    {
        // For Gemini, we don't need to do anything special to end a conversation
        return Task.CompletedTask;
    }

    // IGeminiChatService implementation
    public async Task<SimpleChatResponse> GenerateChatResponseAsync(string message, string? context = null)
    {
        var response = await GenerateResponseAsync(message, context);
        return new SimpleChatResponse
        {
            Message = response,
            IsSuccessful = !response.StartsWith("Error") && !response.StartsWith("Exception"),
            Timestamp = DateTime.UtcNow,
            Source = "Gemini"
        };
    }

    public async Task<SimpleChatResponse> GenerateContextualResponseAsync(string message, List<ChatMessage> conversationHistory)
    {
        var contextBuilder = new StringBuilder();
        foreach (var msg in conversationHistory.TakeLast(5)) // Use last 5 messages for context
        {
            contextBuilder.AppendLine($"{msg.Role}: {msg.Content}");
        }
        contextBuilder.AppendLine($"User: {message}");

        var response = await GenerateResponseAsync(contextBuilder.ToString());
        return new SimpleChatResponse
        {
            Message = response,
            IsSuccessful = !response.StartsWith("Error") && !response.StartsWith("Exception"),
            Timestamp = DateTime.UtcNow,
            Source = "Gemini"
        };
    }

    public async Task<bool> IsServiceAvailableAsync()
    {
        return !string.IsNullOrEmpty(_settings.Gemini.ApiKey);
    }

    public async Task<string> SummarizeConversationAsync(List<ChatMessage> messages)
    {
        var conversationText = string.Join("\n", messages.Select(m => $"{m.Role}: {m.Content}"));
        var summaryPrompt = $"Please summarize this conversation:\n{conversationText}";
        return await GenerateResponseAsync(summaryPrompt);
    }

    public async Task<List<string>> GenerateSuggestionsAsync(string context)
    {
        var prompt = $"Based on this context: {context}\nGenerate 3 helpful suggestions or follow-up questions. Return them as a simple list.";
        var response = await GenerateResponseAsync(prompt);
        
        // Simple parsing - split by lines and take first 3 non-empty lines
        return response.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Take(3)
            .ToList();
    }
}
