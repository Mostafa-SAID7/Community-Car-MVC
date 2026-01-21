using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityCar.Application.Common.Interfaces.Services.AI;
using CommunityCar.AI.Configuration;
using Microsoft.Extensions.Options;

namespace CommunityCar.AI.Services;

public class GeminiChatService : IAIChatService
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

        var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key={apiKey}";

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
}
