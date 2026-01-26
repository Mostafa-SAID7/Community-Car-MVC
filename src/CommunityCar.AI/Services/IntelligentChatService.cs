using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CommunityCar.AI.Configuration;
using CommunityCar.AI.Models;
using System.Text.Json;

namespace CommunityCar.AI.Services;

/// <summary>
/// Intelligent chat service with ML-powered features
/// </summary>
public interface IIntelligentChatService
{
    Task<ChatResponse> ProcessMessageAsync(ChatRequest request);
    Task<List<string>> GetSmartSuggestionsAsync(string partialMessage, string userId);
    Task<ChatModerationResult> ModerateChatAsync(string message, string userId);
    Task<List<ChatInsight>> GetChatInsightsAsync(string conversationId);
    Task<string> GenerateAutoResponseAsync(string message, string context);
    Task<ChatSummary> SummarizeConversationAsync(string conversationId);
    Task<List<string>> ExtractActionItemsAsync(string conversationText);
    Task<ChatTranslation> TranslateMessageAsync(string message, string targetLanguage);
    
    // Additional methods for controller compatibility
    Task<ChatResponse> GetResponseAsync(string message, Guid userId, string? context = null);
    Task<object?> GetConversationHistoryAsync(Guid conversationId, Guid userId);
    Task<List<object>> GetUserConversationsAsync(Guid userId, int page, int pageSize);
    Task<object> StartConversationAsync(Guid userId, string? title, string? context);
    Task<bool> DeleteConversationAsync(Guid conversationId, Guid userId);
    Task<List<string>> GetSuggestionsAsync(string context, Guid userId, int maxSuggestions);
    Task ProcessFeedbackAsync(Guid conversationId, Guid messageId, int rating, string? feedback, Guid userId);
    Task<object> GetUserChatStatsAsync(Guid userId);
}

public class IntelligentChatService : IIntelligentChatService
{
    private readonly ILogger<IntelligentChatService> _logger;
    private readonly AISettings _settings;
    private readonly ISentimentAnalysisService _sentimentService;
    private readonly IPredictionService _predictionService;
    private readonly IGeminiChatService _geminiService;
    private readonly IHuggingFaceChatService _huggingFaceService;

    public IntelligentChatService(
        ILogger<IntelligentChatService> logger,
        IOptions<AISettings> settings,
        ISentimentAnalysisService sentimentService,
        IPredictionService predictionService,
        IGeminiChatService geminiService,
        IHuggingFaceChatService huggingFaceService)
    {
        _logger = logger;
        _settings = settings.Value;
        _sentimentService = sentimentService;
        _predictionService = predictionService;
        _geminiService = geminiService;
        _huggingFaceService = huggingFaceService;
    }

    public async Task<ChatResponse> ProcessMessageAsync(ChatRequest request)
    {
        try
        {
            _logger.LogInformation("Processing chat message from user {UserId}", request.UserId);

            var response = new ChatResponse
            {
                MessageId = Guid.NewGuid().ToString(),
                UserId = request.UserId,
                Timestamp = DateTime.UtcNow
            };

            // Analyze sentiment
            var sentimentAnalysis = await _sentimentService.AnalyzeSentimentAsync(request.Message);
            response.SentimentAnalysis = sentimentAnalysis;

            // Moderate content
            var moderationResult = await ModerateChatAsync(request.Message, request.UserId);
            response.ModerationResult = moderationResult;

            if (moderationResult.IsBlocked)
            {
                response.Message = "Your message has been blocked due to policy violations.";
                response.IsBlocked = true;
                return response;
            }

            // Generate intelligent response
            response.Message = await GenerateIntelligentResponseAsync(request);

            // Get smart suggestions for follow-up
            response.Suggestions = await GetSmartSuggestionsAsync(request.Message, request.UserId);

            // Predict engagement
            response.EngagementScore = await _predictionService.PredictEngagementAsync(request.Message, request.UserId);

            // Extract entities and topics
            response.ExtractedTopics = await ExtractTopicsAsync(request.Message);
            response.ExtractedEntities = await ExtractEntitiesAsync(request.Message);

            _logger.LogInformation("Processed chat message successfully for user {UserId}", request.UserId);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message for user {UserId}", request.UserId);
            return new ChatResponse
            {
                MessageId = Guid.NewGuid().ToString(),
                UserId = request.UserId,
                Message = "I'm sorry, I encountered an error processing your message. Please try again.",
                Timestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<List<string>> GetSmartSuggestionsAsync(string partialMessage, string userId)
    {
        try
        {
            var suggestions = new List<string>();

            // Get user interests for personalized suggestions
            var userInterests = await _predictionService.GetUserInterestScoresAsync(userId);
            var topInterests = userInterests.OrderByDescending(x => x.Value).Take(3).Select(x => x.Key).ToList();

            // Context-aware suggestions based on partial message
            var lowerMessage = partialMessage.ToLower();

            if (lowerMessage.Contains("car") || lowerMessage.Contains("vehicle"))
            {
                suggestions.AddRange(new[]
                {
                    "What's the best maintenance schedule for my car?",
                    "How can I improve my car's fuel efficiency?",
                    "What are the signs of engine problems?",
                    "Which car insurance is recommended?",
                    "How often should I change my oil?"
                });
            }
            else if (lowerMessage.Contains("problem") || lowerMessage.Contains("issue"))
            {
                suggestions.AddRange(new[]
                {
                    "Can you help me diagnose this issue?",
                    "What could be causing this problem?",
                    "Is this something I can fix myself?",
                    "Should I take it to a mechanic?",
                    "How urgent is this repair?"
                });
            }
            else if (lowerMessage.Contains("buy") || lowerMessage.Contains("purchase"))
            {
                suggestions.AddRange(new[]
                {
                    "What should I look for when buying a used car?",
                    "Which car model is best for my needs?",
                    "How do I negotiate the best price?",
                    "What documents do I need for purchase?",
                    "Should I get a pre-purchase inspection?"
                });
            }
            else
            {
                // General automotive suggestions
                suggestions.AddRange(new[]
                {
                    "Tell me about electric vehicles",
                    "How can I improve my driving skills?",
                    "What are the latest automotive technologies?",
                    "Share some car maintenance tips",
                    "Help me understand car insurance options"
                });
            }

            // Add personalized suggestions based on user interests
            foreach (var interest in topInterests)
            {
                suggestions.Add($"Tell me more about {interest.ToLower()}");
            }

            return suggestions.Distinct().Take(8).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting smart suggestions for user {UserId}", userId);
            return new List<string>();
        }
    }

    public async Task<ChatModerationResult> ModerateChatAsync(string message, string userId)
    {
        try
        {
            var result = new ChatModerationResult
            {
                MessageId = Guid.NewGuid().ToString(),
                UserId = userId,
                OriginalMessage = message
            };

            // Check toxicity
            var toxicityScore = await _sentimentService.GetToxicityScoreAsync(message);
            result.ToxicityScore = toxicityScore;

            // Check for spam
            result.IsSpam = await _sentimentService.IsSpamAsync(message);

            // Check for inappropriate content
            result.HasInappropriateContent = await CheckInappropriateContentAsync(message);

            // Determine if message should be blocked
            result.IsBlocked = toxicityScore > 0.7f || result.IsSpam || result.HasInappropriateContent;

            // Generate moderation reasons
            if (result.IsBlocked)
            {
                result.ModerationReasons = new List<string>();
                if (toxicityScore > 0.7f) result.ModerationReasons.Add("High toxicity detected");
                if (result.IsSpam) result.ModerationReasons.Add("Spam content detected");
                if (result.HasInappropriateContent) result.ModerationReasons.Add("Inappropriate content detected");
            }

            // Suggest alternative if blocked
            if (result.IsBlocked)
            {
                result.SuggestedAlternative = await GenerateAlternativeMessageAsync(message);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moderating chat message");
            return new ChatModerationResult
            {
                MessageId = Guid.NewGuid().ToString(),
                UserId = userId,
                OriginalMessage = message,
                IsBlocked = false
            };
        }
    }

    public async Task<List<ChatInsight>> GetChatInsightsAsync(string conversationId)
    {
        try
        {
            var insights = new List<ChatInsight>();

            // This would analyze the conversation history
            // For now, return sample insights
            insights.Add(new ChatInsight
            {
                Type = "Sentiment Trend",
                Description = "Overall conversation sentiment is positive with increasing engagement",
                Confidence = 0.85f,
                Timestamp = DateTime.UtcNow
            });

            insights.Add(new ChatInsight
            {
                Type = "Topic Analysis",
                Description = "Main topics discussed: Car maintenance, Insurance, Electric vehicles",
                Confidence = 0.92f,
                Timestamp = DateTime.UtcNow
            });

            insights.Add(new ChatInsight
            {
                Type = "User Engagement",
                Description = "High user engagement with technical questions receiving most responses",
                Confidence = 0.78f,
                Timestamp = DateTime.UtcNow
            });

            return insights;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chat insights for conversation {ConversationId}", conversationId);
            return new List<ChatInsight>();
        }
    }

    public async Task<string> GenerateAutoResponseAsync(string message, string context)
    {
        try
        {
            // Use Gemini or HuggingFace for response generation
            var prompt = $"Context: {context}\nUser message: {message}\nGenerate a helpful, friendly response for a car community platform:";
            
            try
            {
                var response = await _geminiService.GenerateChatResponseAsync(prompt);
                return response.Message;
            }
            catch
            {
                // Fallback to HuggingFace
                var response = await _huggingFaceService.GenerateChatResponseAsync(prompt);
                return response.Message;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating auto response");
            return "Thank you for your message. How can I help you with your automotive needs today?";
        }
    }

    public async Task<ChatSummary> SummarizeConversationAsync(string conversationId)
    {
        try
        {
            // This would analyze the full conversation
            // For now, return a sample summary
            return new ChatSummary
            {
                ConversationId = conversationId,
                Summary = "Discussion about car maintenance schedules and best practices for engine care. User received recommendations for oil change intervals and tire rotation.",
                KeyTopics = new List<string> { "Maintenance", "Oil Change", "Tire Care", "Engine Health" },
                ParticipantCount = 3,
                MessageCount = 15,
                Duration = TimeSpan.FromMinutes(45),
                OverallSentiment = SentimentType.Positive,
                ActionItems = new List<string> { "Schedule oil change", "Check tire pressure", "Research maintenance schedule" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error summarizing conversation {ConversationId}", conversationId);
            return new ChatSummary
            {
                ConversationId = conversationId,
                Summary = "Unable to generate summary",
                KeyTopics = new List<string>(),
                ActionItems = new List<string>()
            };
        }
    }

    public async Task<List<string>> ExtractActionItemsAsync(string conversationText)
    {
        try
        {
            var actionItems = new List<string>();
            var lines = conversationText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var lowerLine = line.ToLower();
                
                // Look for action-oriented phrases
                if (lowerLine.Contains("need to") || lowerLine.Contains("should") || 
                    lowerLine.Contains("must") || lowerLine.Contains("have to") ||
                    lowerLine.Contains("remember to") || lowerLine.Contains("don't forget"))
                {
                    actionItems.Add(line.Trim());
                }
                
                // Look for scheduling phrases
                if (lowerLine.Contains("schedule") || lowerLine.Contains("appointment") ||
                    lowerLine.Contains("book") || lowerLine.Contains("call"))
                {
                    actionItems.Add(line.Trim());
                }
            }

            return await Task.FromResult(actionItems.Distinct().Take(10).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting action items");
            return new List<string>();
        }
    }

    public async Task<ChatTranslation> TranslateMessageAsync(string message, string targetLanguage)
    {
        try
        {
            // This would use a translation service
            // For now, return a simple response
            var translation = new ChatTranslation
            {
                OriginalMessage = message,
                OriginalLanguage = "en",
                TargetLanguage = targetLanguage,
                TranslatedMessage = message, // Would be actual translation
                Confidence = 0.95f
            };

            // Simple Arabic translation examples for common phrases
            if (targetLanguage == "ar" && message.ToLower().Contains("hello"))
            {
                translation.TranslatedMessage = "مرحبا";
            }
            else if (targetLanguage == "ar" && message.ToLower().Contains("thank you"))
            {
                translation.TranslatedMessage = "شكرا لك";
            }

            return await Task.FromResult(translation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating message");
            return new ChatTranslation
            {
                OriginalMessage = message,
                OriginalLanguage = "en",
                TargetLanguage = targetLanguage,
                TranslatedMessage = message,
                Confidence = 0.0f
            };
        }
    }

    private async Task<string> GenerateIntelligentResponseAsync(ChatRequest request)
    {
        try
        {
            // Analyze the message type and generate appropriate response
            var messageType = await DetermineMessageTypeAsync(request.Message);
            
            return messageType switch
            {
                MessageType.Question => await GenerateAnswerAsync(request.Message),
                MessageType.Problem => await GenerateProblemSolutionAsync(request.Message),
                MessageType.Greeting => await GenerateGreetingResponseAsync(request.UserId),
                MessageType.Appreciation => "You're welcome! I'm here to help with any automotive questions you have.",
                MessageType.Complaint => await GenerateEmpathyResponseAsync(request.Message),
                _ => await GenerateGeneralResponseAsync(request.Message)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating intelligent response");
            return "I'm here to help! What would you like to know about cars and automotive topics?";
        }
    }

    private async Task<MessageType> DetermineMessageTypeAsync(string message)
    {
        var lowerMessage = message.ToLower();
        
        if (lowerMessage.Contains("?") || lowerMessage.StartsWith("what") || lowerMessage.StartsWith("how") || 
            lowerMessage.StartsWith("why") || lowerMessage.StartsWith("when") || lowerMessage.StartsWith("where"))
            return MessageType.Question;
            
        if (lowerMessage.Contains("problem") || lowerMessage.Contains("issue") || lowerMessage.Contains("broken") ||
            lowerMessage.Contains("not working") || lowerMessage.Contains("error"))
            return MessageType.Problem;
            
        if (lowerMessage.Contains("hello") || lowerMessage.Contains("hi") || lowerMessage.Contains("hey") ||
            lowerMessage.Contains("good morning") || lowerMessage.Contains("good afternoon"))
            return MessageType.Greeting;
            
        if (lowerMessage.Contains("thank") || lowerMessage.Contains("thanks") || lowerMessage.Contains("appreciate"))
            return MessageType.Appreciation;
            
        if (lowerMessage.Contains("terrible") || lowerMessage.Contains("awful") || lowerMessage.Contains("hate") ||
            lowerMessage.Contains("disappointed") || lowerMessage.Contains("frustrated"))
            return MessageType.Complaint;
            
        return await Task.FromResult(MessageType.General);
    }

    private async Task<string> GenerateAnswerAsync(string question)
    {
        // This would use AI to generate contextual answers
        // For now, return template responses based on keywords
        var lowerQuestion = question.ToLower();
        
        if (lowerQuestion.Contains("oil change"))
            return "Most vehicles need an oil change every 3,000-5,000 miles, but check your owner's manual for specific recommendations. Synthetic oil can often go longer between changes.";
            
        if (lowerQuestion.Contains("tire pressure"))
            return "Check your tire pressure monthly when tires are cold. The recommended pressure is usually found on a sticker inside the driver's door or in your owner's manual.";
            
        if (lowerQuestion.Contains("battery"))
            return "Car batteries typically last 3-5 years. Signs of a failing battery include slow engine cranking, dim headlights, and the battery warning light on your dashboard.";
            
        return await Task.FromResult("That's a great question! Let me help you find the information you need. Could you provide more specific details about your situation?");
    }

    private async Task<string> GenerateProblemSolutionAsync(string problem)
    {
        var lowerProblem = problem.ToLower();
        
        if (lowerProblem.Contains("won't start"))
            return "If your car won't start, check: 1) Battery connections, 2) Fuel level, 3) Listen for clicking sounds when turning the key. If you hear clicking, it's likely the battery.";
            
        if (lowerProblem.Contains("overheating"))
            return "If your car is overheating: 1) Pull over safely, 2) Turn off the engine, 3) Wait for it to cool down, 4) Check coolant levels when cool. Don't remove the radiator cap when hot!";
            
        if (lowerProblem.Contains("strange noise"))
            return "Strange noises can indicate various issues. Can you describe the noise? Is it squealing, grinding, clicking, or knocking? Also, when does it occur - when starting, braking, or driving?";
            
        return await Task.FromResult("I understand you're experiencing an issue. For safety reasons, I recommend having a qualified mechanic diagnose the problem. Can you describe the symptoms in more detail?");
    }

    private async Task<string> GenerateGreetingResponseAsync(string userId)
    {
        var greetings = new[]
        {
            "Hello! Welcome to the Community Car platform. How can I assist you with your automotive needs today?",
            "Hi there! I'm here to help with any car-related questions or issues you might have.",
            "Welcome! Whether you need maintenance tips, troubleshooting help, or general automotive advice, I'm here to help.",
            "Hello! Ready to help you with anything car-related. What's on your mind today?"
        };
        
        var random = new Random();
        return await Task.FromResult(greetings[random.Next(greetings.Length)]);
    }

    private async Task<string> GenerateEmpathyResponseAsync(string complaint)
    {
        return await Task.FromResult("I understand your frustration, and I'm sorry you're experiencing this issue. Let me help you find a solution. Can you tell me more about what's happening?");
    }

    private async Task<string> GenerateGeneralResponseAsync(string message)
    {
        return await Task.FromResult("Thanks for sharing! I'm here to help with any automotive questions or concerns you might have. Is there something specific I can assist you with?");
    }

    private async Task<bool> CheckInappropriateContentAsync(string message)
    {
        var inappropriateWords = new[] { "explicit", "inappropriate", "offensive" }; // This would be a more comprehensive list
        var lowerMessage = message.ToLower();
        
        return await Task.FromResult(inappropriateWords.Any(word => lowerMessage.Contains(word)));
    }

    private async Task<string> GenerateAlternativeMessageAsync(string originalMessage)
    {
        return await Task.FromResult("Perhaps you could rephrase your message in a more constructive way? I'm here to help with your automotive needs.");
    }

    private async Task<List<string>> ExtractTopicsAsync(string message)
    {
        var topics = new List<string>();
        var lowerMessage = message.ToLower();
        
        var topicKeywords = new Dictionary<string, string[]>
        {
            { "Maintenance", new[] { "oil", "change", "service", "maintenance", "repair", "fix" } },
            { "Engine", new[] { "engine", "motor", "horsepower", "performance", "turbo" } },
            { "Tires", new[] { "tire", "wheel", "pressure", "rotation", "alignment" } },
            { "Insurance", new[] { "insurance", "coverage", "claim", "policy", "premium" } },
            { "Electric Vehicles", new[] { "electric", "ev", "hybrid", "battery", "charging" } },
            { "Safety", new[] { "safety", "accident", "airbag", "seatbelt", "crash" } }
        };
        
        foreach (var topic in topicKeywords)
        {
            if (topic.Value.Any(keyword => lowerMessage.Contains(keyword)))
            {
                topics.Add(topic.Key);
            }
        }
        
        return await Task.FromResult(topics);
    }

    private async Task<List<string>> ExtractEntitiesAsync(string message)
    {
        var entities = new List<string>();
        
        // Simple entity extraction (would use NLP in production)
        var words = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var word in words)
        {
            // Look for car brands, models, etc.
            if (IsCarBrand(word) || IsCarModel(word))
            {
                entities.Add(word);
            }
        }
        
        return await Task.FromResult(entities.Distinct().ToList());
    }

    private bool IsCarBrand(string word)
    {
        var brands = new[] { "toyota", "honda", "ford", "chevrolet", "bmw", "mercedes", "audi", "volkswagen", "nissan", "hyundai" };
        return brands.Contains(word.ToLower());
    }

    private bool IsCarModel(string word)
    {
        var models = new[] { "camry", "accord", "civic", "corolla", "f150", "mustang", "prius", "altima", "elantra" };
        return models.Contains(word.ToLower());
    }

    // Additional methods for controller compatibility
    public async Task<ChatResponse> GetResponseAsync(string message, Guid userId, string? context = null)
    {
        var conversationId = Guid.NewGuid();
        var request = new ChatRequest
        {
            UserId = userId.ToString(),
            Message = message,
            Context = context ?? string.Empty,
            ConversationId = conversationId.ToString()
        };

        var response = await ProcessMessageAsync(request);
        
        // Ensure the response has the expected properties for the controller
        response.MessageId = Guid.NewGuid().ToString();
        response.ConversationId = conversationId;
        response.Confidence = 0.85f; // Default confidence score
        
        return response;
    }

    public async Task<object?> GetConversationHistoryAsync(Guid conversationId, Guid userId)
    {
        // For now, return a simple conversation history
        // In a real implementation, this would fetch from database
        return await Task.FromResult(new
        {
            ConversationId = conversationId,
            UserId = userId,
            Messages = new List<object>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
    }

    public async Task<List<object>> GetUserConversationsAsync(Guid userId, int page, int pageSize)
    {
        // For now, return empty list
        // In a real implementation, this would fetch from database
        return await Task.FromResult(new List<object>());
    }

    public async Task<object> StartConversationAsync(Guid userId, string? title, string? context)
    {
        var conversationId = Guid.NewGuid();
        return await Task.FromResult(new
        {
            ConversationId = conversationId,
            UserId = userId,
            Title = title ?? "New Conversation",
            Context = context ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task<bool> DeleteConversationAsync(Guid conversationId, Guid userId)
    {
        // For now, always return true
        // In a real implementation, this would delete from database
        return await Task.FromResult(true);
    }

    public async Task<List<string>> GetSuggestionsAsync(string context, Guid userId, int maxSuggestions)
    {
        return await GetSmartSuggestionsAsync(context, userId.ToString());
    }

    public async Task ProcessFeedbackAsync(Guid conversationId, Guid messageId, int rating, string? feedback, Guid userId)
    {
        // For now, just log the feedback
        // In a real implementation, this would store feedback for ML training
        _logger.LogInformation("Received feedback for conversation {ConversationId}, message {MessageId}: Rating {Rating}, Feedback: {Feedback}", 
            conversationId, messageId, rating, feedback);
        await Task.CompletedTask;
    }

    public async Task<object> GetUserChatStatsAsync(Guid userId)
    {
        return await Task.FromResult(new
        {
            UserId = userId,
            TotalConversations = 0,
            TotalMessages = 0,
            AverageResponseTime = TimeSpan.Zero,
            MostActiveHour = 12,
            FavoriteTopics = new List<string> { "Car Maintenance", "Insurance", "Electric Vehicles" },
            SentimentTrend = "Positive"
        });
    }
}

// Supporting classes
public class ChatRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ConversationId { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ChatResponse
{
    public string MessageId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }
    public float Confidence { get; set; }
    public Guid ConversationId { get; set; }
    public EnhancedSentimentPrediction? SentimentAnalysis { get; set; }
    public ChatModerationResult? ModerationResult { get; set; }
    public List<string> Suggestions { get; set; } = new();
    public float EngagementScore { get; set; }
    public List<string> ExtractedTopics { get; set; } = new();
    public List<string> ExtractedEntities { get; set; } = new();
    public DateTime Timestamp { get; set; }
}

public class ChatModerationResult
{
    public string MessageId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string OriginalMessage { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }
    public bool IsSpam { get; set; }
    public bool HasInappropriateContent { get; set; }
    public float ToxicityScore { get; set; }
    public List<string> ModerationReasons { get; set; } = new();
    public string SuggestedAlternative { get; set; } = string.Empty;
}

public class ChatInsight
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ChatSummary
{
    public string ConversationId { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<string> KeyTopics { get; set; } = new();
    public int ParticipantCount { get; set; }
    public int MessageCount { get; set; }
    public TimeSpan Duration { get; set; }
    public SentimentType OverallSentiment { get; set; }
    public List<string> ActionItems { get; set; } = new();
}

public class ChatTranslation
{
    public string OriginalMessage { get; set; } = string.Empty;
    public string OriginalLanguage { get; set; } = string.Empty;
    public string TargetLanguage { get; set; } = string.Empty;
    public string TranslatedMessage { get; set; } = string.Empty;
    public float Confidence { get; set; }
}

public enum MessageType
{
    General,
    Question,
    Problem,
    Greeting,
    Appreciation,
    Complaint
}

// Additional models for controller compatibility
public class ChatMessageResponse
{
    public string Message { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public List<string> Suggestions { get; set; } = new();
    public Guid ConversationId { get; set; }
}