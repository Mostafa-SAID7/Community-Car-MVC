using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityCar.Application.Common.Interfaces.Services.AI;
using CommunityCar.AI.Configuration;
using Microsoft.Extensions.Options;

namespace CommunityCar.AI.Services;

public class HuggingFaceChatService : IAIChatService
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
}
