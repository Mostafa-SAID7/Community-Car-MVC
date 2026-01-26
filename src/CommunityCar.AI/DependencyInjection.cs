using System.Net.Http;
using CommunityCar.AI.Configuration;
using CommunityCar.AI.Services;
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

        // Register individual AI services (scoped because they use HttpClient)
        services.AddScoped<IGeminiChatService, GeminiChatService>();
        services.AddScoped<IHuggingFaceChatService, HuggingFaceChatService>();
        
        // Register ML.NET services
        services.AddSingleton<ISentimentAnalysisService, SentimentAnalysisService>();
        services.AddSingleton<IPredictionService, PredictionService>();
        services.AddScoped<IIntelligentChatService, IntelligentChatService>();
        services.AddScoped<IMLPipelineService, MLPipelineService>();

        return services;
    }
}
