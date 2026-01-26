using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityCar.Application.Common.Interfaces.Services.AI;
using CommunityCar.AI.Configuration;
using CommunityCar.AI.Models;
using Microsoft.Extensions.Options;

namespace CommunityCar.AI.Services;

public class HuggingFaceChatService : IAIChatService, IHuggingFaceChatService
{
    private readonly HttpClient _httpClient;
    private readonly AISettings _settings;

    public HuggingFaceChatService(HttpClient httpClient, IOptions<AISettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<string> GenerateResponseAsync(string message, string? conversationId = null)
    {
        var apiKey = _settings.HuggingFace.ApiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            return "Error: HuggingFace API Key is missing.";
        }
        
        // Using a generic inference API endpoint - user might need to specify model URL
        var url = "https://api-inference.huggingface.co/models/gpt2"; // Default fallback or use config
        if (!string.IsNullOrEmpty(_settings.HuggingFace.ModelUrl))
        {
            url = _settings.HuggingFace.ModelUrl;
        }

        var requestBody = new { inputs = message };
        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        try
        {
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            return responseString; // HuggingFace format varies by model, returning raw for now
        }
        catch (Exception ex)
        {
            return $"Error communicating with HuggingFace: {ex.Message}";
        }
    }

    public Task<string> StartConversationAsync()
    {
        // Generate a simple conversation ID for HuggingFace
        var conversationId = Guid.NewGuid().ToString();
        return Task.FromResult(conversationId);
    }

    public Task EndConversationAsync(string conversationId)
    {
        // For HuggingFace, we don't need to do anything special to end a conversation
        return Task.CompletedTask;
    }

    // IHuggingFaceChatService implementation
    public async Task<ChatResponse> GenerateChatResponseAsync(string message, string? context = null)
    {
        var response = await GenerateResponseAsync(message, context);
        return new ChatResponse
        {
            Message = response,
            IsSuccessful = !response.StartsWith("Error"),
            Timestamp = DateTime.UtcNow,
            Source = "HuggingFace"
        };
    }

    public async Task<ChatResponse> GenerateContextualResponseAsync(string message, List<ChatMessage> conversationHistory)
    {
        var contextBuilder = new StringBuilder();
        foreach (var msg in conversationHistory.TakeLast(5))
        {
            contextBuilder.AppendLine($"{msg.Role}: {msg.Content}");
        }
        contextBuilder.AppendLine($"User: {message}");

        var response = await GenerateResponseAsync(contextBuilder.ToString());
        return new ChatResponse
        {
            Message = response,
            IsSuccessful = !response.StartsWith("Error"),
            Timestamp = DateTime.UtcNow,
            Source = "HuggingFace"
        };
    }

    public async Task<bool> IsServiceAvailableAsync()
    {
        return !string.IsNullOrEmpty(_settings.HuggingFace.ApiKey);
    }

    public async Task<string> SummarizeConversationAsync(List<ChatMessage> messages)
    {
        var conversationText = string.Join("\n", messages.Select(m => $"{m.Role}: {m.Content}"));
        var summaryPrompt = $"Summarize: {conversationText}";
        return await GenerateResponseAsync(summaryPrompt);
    }

    public async Task<List<string>> GenerateSuggestionsAsync(string context)
    {
        var prompt = $"Context: {context}\nSuggest 3 follow-up questions:";
        var response = await GenerateResponseAsync(prompt);
        
        return response.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Take(3)
            .ToList();
    }
}
