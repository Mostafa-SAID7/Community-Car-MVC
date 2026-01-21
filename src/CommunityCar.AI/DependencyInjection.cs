using System.Net.Http;
using CommunityCar.AI.Configuration;
using CommunityCar.AI.Services;
using CommunityCar.Application.Common.Interfaces.Services.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityCar.AI;

public static class DependencyInjection
{
    public static IServiceCollection AddAIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AISettings>(configuration.GetSection(AISettings.SectionName));

        var aiSettings = configuration.GetSection(AISettings.SectionName).Get<AISettings>() ?? new AISettings();

        // Register HttpClient for services
        services.AddHttpClient<GeminiChatService>();
        services.AddHttpClient<HuggingFaceChatService>();

        // Register the appropriate service based on default provider
        if (aiSettings.DefaultProvider.Equals("HuggingFace", System.StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IAIChatService, HuggingFaceChatService>();
        }
        else
        {
            // Default to Gemini
            services.AddScoped<IAIChatService, GeminiChatService>();
        }

        return services;
    }
}
