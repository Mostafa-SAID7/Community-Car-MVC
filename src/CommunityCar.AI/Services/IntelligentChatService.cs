using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CommunityCar.AI.Configuration;
using CommunityCar.AI.Models;
using System.Text.Json;
using static CommunityCar.AI.Models.SentimentData;
using CommunityCar.Application.Services.Community;
using CommunityCar.Application.Features.Community.News.ViewModels;

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
            response.SentimentAnalysis = new EnhancedSentimentPrediction
            {
                Label = sentimentAnalysis.Label,
                Score = sentimentAnalysis.Score,
                Confidence = sentimentAnalysis.Confidence,
                Emotions = sentimentAnalysis.Emotions,
                Keywords = new List<string>(), // Will be populated by sentiment service
                Context = "Car Community"
            };

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
            var lowerMessage = partialMessage.ToLower();

            // Context-aware suggestions based on partial message
            if (lowerMessage.Contains("price") || lowerMessage.Contains("cost") || lowerMessage.Contains("expensive") || lowerMessage.Contains("budget"))
            {
                suggestions.AddRange(new[]
                {
                    "What are typical oil change costs in my area?",
                    "Compare tire prices from community members",
                    "Find budget-friendly brake service options",
                    "Get insurance cost comparisons and recommendations",
                    "Explore group buying opportunities for parts"
                });
            }
            else if (lowerMessage.Contains("maintenance") || lowerMessage.Contains("service") || lowerMessage.Contains("schedule"))
            {
                suggestions.AddRange(new[]
                {
                    "Show me maintenance schedules for my vehicle",
                    "Find community-recommended service providers",
                    "Get seasonal maintenance checklists and tips",
                    "Learn about DIY maintenance from community guides",
                    "Track my vehicle's service history and reminders"
                });
            }
            else if (lowerMessage.Contains("part") || lowerMessage.Contains("parts") || lowerMessage.Contains("buy") || lowerMessage.Contains("sell"))
            {
                suggestions.AddRange(new[]
                {
                    "Browse community marketplace for quality parts",
                    "Find OEM vs aftermarket part recommendations",
                    "Get part numbers and compatibility information",
                    "Connect with members selling specific parts",
                    "Join group purchases for better part pricing"
                });
            }
            else if (lowerMessage.Contains("map") || lowerMessage.Contains("near") || lowerMessage.Contains("location") || lowerMessage.Contains("find"))
            {
                suggestions.AddRange(new[]
                {
                    "Find highly-rated mechanics near me",
                    "Locate auto parts stores with community reviews",
                    "Discover gas stations with current fuel prices",
                    "Map electric vehicle charging stations",
                    "Find car washes and detailing services nearby"
                });
            }
            else if (lowerMessage.Contains("problem") || lowerMessage.Contains("issue") || lowerMessage.Contains("broken") || lowerMessage.Contains("fix"))
            {
                suggestions.AddRange(new[]
                {
                    "Diagnose my car's starting problems",
                    "Get help with unusual engine noises",
                    "Find solutions for brake issues",
                    "Troubleshoot electrical problems with community help",
                    "Connect with members who solved similar issues"
                });
            }
            else if (lowerMessage.Contains("car") || lowerMessage.Contains("vehicle") || lowerMessage.Contains("auto"))
            {
                suggestions.AddRange(new[]
                {
                    "Get maintenance tips for my specific car model",
                    "Find community discussions about my vehicle",
                    "Compare service costs for my car type",
                    "Discover performance upgrades and modifications",
                    "Join model-specific community groups"
                });
            }
            else if (lowerMessage.Contains("community") || lowerMessage.Contains("member") || lowerMessage.Contains("forum"))
            {
                suggestions.AddRange(new[]
                {
                    "Explore community forums and discussions",
                    "Connect with local automotive enthusiasts",
                    "Share my automotive experiences and reviews",
                    "Find community events and meetups",
                    "Join specialized automotive interest groups"
                });
            }
            else
            {
                // General automotive community suggestions
                suggestions.AddRange(new[]
                {
                    "Find trusted mechanics with community reviews",
                    "Get maintenance schedules and cost estimates",
                    "Browse community marketplace for parts deals",
                    "Explore local automotive services on our maps",
                    "Connect with experienced community members",
                    "Learn DIY maintenance from community guides",
                    "Compare service pricing from member experiences",
                    "Join discussions about your vehicle model"
                });
            }

            // Get user interests for personalized suggestions
            try
            {
                var userInterests = await _predictionService.GetUserInterestScoresAsync(userId);
                var topInterests = userInterests.OrderByDescending(x => x.Value).Take(2).Select(x => x.Key).ToList();

                // Add personalized suggestions based on user interests
                foreach (var interest in topInterests)
                {
                    suggestions.Add($"Explore {interest.ToLower()} resources in our community");
                }
            }
            catch
            {
                // If user interest prediction fails, continue with general suggestions
            }

            return suggestions.Distinct().Take(8).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting smart suggestions for user {UserId}", userId);
            return new List<string>
            {
                "Find trusted automotive services near you",
                "Get maintenance guidance for your vehicle",
                "Explore community marketplace for parts",
                "Connect with automotive enthusiasts",
                "Learn from community repair guides"
            };
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
            result.ToxicityScore = (float)toxicityScore;

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
        var lowerQuestion = question.ToLower();
        
        // Pricing-related questions
        if (lowerQuestion.Contains("price") || lowerQuestion.Contains("cost") || lowerQuestion.Contains("expensive") || 
            lowerQuestion.Contains("cheap") || lowerQuestion.Contains("budget") || lowerQuestion.Contains("afford"))
        {
            if (lowerQuestion.Contains("oil change"))
                return "Oil change prices typically range from $25-$80 depending on oil type and location. Conventional oil is cheapest ($25-40), while full synthetic runs $50-80. Many community members recommend checking local shops for deals and reading reviews in our community before choosing a service provider.";
            
            if (lowerQuestion.Contains("tire") || lowerQuestion.Contains("tires"))
                return "Tire prices vary widely based on size, brand, and type. Budget tires start around $50-80 each, mid-range $80-150, and premium $150-300+. Check our community marketplace for member recommendations and potential group buying opportunities for better deals.";
            
            if (lowerQuestion.Contains("brake") || lowerQuestion.Contains("brakes"))
                return "Brake service costs depend on what needs replacement. Brake pads typically cost $100-300 per axle, rotors $200-400 per axle. Our community members often share trusted mechanic recommendations and fair pricing experiences in the reviews section.";
            
            if (lowerQuestion.Contains("insurance"))
                return "Car insurance costs vary by location, driving record, and coverage. Average monthly costs range from $100-200. Many community members discuss their insurance experiences and recommendations in our forums. Consider getting quotes from multiple providers.";
            
            return "Automotive costs can vary significantly by location and service provider. I'd recommend checking our community reviews and marketplace for member experiences and recommendations. What specific service or part are you looking to price?";
        }
        
        // Maintenance-related questions
        if (lowerQuestion.Contains("maintenance") || lowerQuestion.Contains("service") || lowerQuestion.Contains("schedule") ||
            lowerQuestion.Contains("when to") || lowerQuestion.Contains("how often"))
        {
            if (lowerQuestion.Contains("oil"))
                return "Oil change intervals depend on your vehicle and oil type:\n• Conventional oil: Every 3,000-5,000 miles\n• Synthetic blend: Every 5,000-7,500 miles\n• Full synthetic: Every 7,500-10,000 miles\n\nCheck your owner's manual for specific recommendations. Many community members track their maintenance in our service logs feature.";
            
            if (lowerQuestion.Contains("tire"))
                return "Tire maintenance schedule:\n• Pressure check: Monthly\n• Rotation: Every 5,000-8,000 miles\n• Alignment check: Annually or if you notice uneven wear\n• Replacement: When tread depth reaches 2/32\"\n\nOur community has a tire maintenance tracker where members share their experiences and tips.";
            
            if (lowerQuestion.Contains("brake"))
                return "Brake maintenance guidelines:\n• Inspection: Every 12,000 miles or annually\n• Pad replacement: Every 25,000-70,000 miles (varies by driving)\n• Fluid change: Every 2-3 years\n• Listen for squealing or grinding sounds\n\nCheck our maintenance guides section for detailed brake care tips from experienced community members.";
            
            if (lowerQuestion.Contains("battery"))
                return "Battery maintenance tips:\n• Test annually after 3 years\n• Clean terminals regularly\n• Check voltage monthly in extreme weather\n• Replace every 3-5 years typically\n\nMany community members share battery testing tips and replacement experiences in our DIY section.";
            
            return "Regular maintenance is key to vehicle longevity! Our community maintenance section has detailed schedules and member experiences for various services. What specific maintenance item are you asking about?";
        }
        
        // Parts-related questions
        if (lowerQuestion.Contains("part") || lowerQuestion.Contains("parts") || lowerQuestion.Contains("replace") ||
            lowerQuestion.Contains("buy") || lowerQuestion.Contains("where to get"))
        {
            if (lowerQuestion.Contains("brake"))
                return "For brake parts, consider:\n• OEM parts for best fit and performance\n• Quality aftermarket brands like Bosch, Wagner, or Akebono\n• Check our community marketplace for member recommendations\n• Many members share part numbers and supplier experiences in our parts forum\n\nWhat specific brake component do you need?";
            
            if (lowerQuestion.Contains("filter"))
                return "Filter replacement options:\n• Air filters: K&N, Fram, or OEM every 12,000-15,000 miles\n• Oil filters: Use manufacturer-recommended specs\n• Cabin filters: Replace every 12,000-15,000 miles\n\nOur community parts section has member reviews and part number databases to help you find the right filters for your vehicle.";
            
            if (lowerQuestion.Contains("battery"))
                return "Battery replacement considerations:\n• Check CCA (Cold Cranking Amps) requirements\n• Popular brands: Interstate, DieHard, Optima\n• Group size must match your vehicle\n• Many auto parts stores offer free installation\n\nCommunity members often share battery experiences and local store recommendations in our reviews section.";
            
            return "Finding the right parts is crucial for proper repairs. Our community has an extensive parts database with member reviews, part numbers, and supplier recommendations. What specific part are you looking for? I can help you connect with community members who have experience with similar repairs.";
        }
        
        // Maps and location-related questions
        if (lowerQuestion.Contains("map") || lowerQuestion.Contains("location") || lowerQuestion.Contains("where") ||
            lowerQuestion.Contains("near me") || lowerQuestion.Contains("directions") || lowerQuestion.Contains("route"))
        {
            if (lowerQuestion.Contains("mechanic") || lowerQuestion.Contains("shop") || lowerQuestion.Contains("garage"))
                return "Finding trusted mechanics near you:\n• Check our interactive map for community-recommended shops\n• Filter by services offered and member ratings\n• Read detailed reviews from community members\n• Many shops offer special discounts to community members\n\nUse our maps feature to find highly-rated automotive services in your area with real member experiences.";
            
            if (lowerQuestion.Contains("gas") || lowerQuestion.Contains("fuel") || lowerQuestion.Contains("station"))
                return "Finding fuel stations:\n• Use our maps to locate gas stations with current prices\n• Community members report fuel prices and station conditions\n• Filter by fuel type (regular, premium, diesel, electric charging)\n• Find stations with amenities like car washes or convenience stores\n\nOur fuel finder helps you locate the best prices and cleanest stations based on community feedback.";
            
            if (lowerQuestion.Contains("parts store") || lowerQuestion.Contains("auto parts"))
                return "Locating auto parts stores:\n• Our maps show nearby parts retailers with community ratings\n• Compare prices and inventory from member experiences\n• Find stores that offer tool lending or installation services\n• Many stores provide special pricing for community members\n\nUse our parts store locator to find the best suppliers in your area with verified community reviews.";
            
            return "Our interactive maps feature helps you find automotive services, parts stores, fuel stations, and more in your area. All locations include community reviews and ratings. What type of automotive service or location are you trying to find?";
        }
        
        // General automotive questions
        if (lowerQuestion.Contains("oil change"))
            return "Oil change essentials:\n• Frequency: Every 3,000-10,000 miles depending on oil type\n• Cost: $25-80 depending on oil and location\n• DIY vs Professional: Community members share both experiences\n• Oil types: Conventional, synthetic blend, full synthetic\n\nCheck our maintenance section for member guides and local shop recommendations with pricing comparisons.";
            
        if (lowerQuestion.Contains("tire pressure"))
            return "Tire pressure management:\n• Check monthly when tires are cold\n• Find recommended PSI on door jamb sticker or manual\n• Under-inflation reduces fuel economy and tire life\n• Over-inflation causes uneven wear\n\nOur community tire care section has pressure monitoring tips and recommended gauge brands from experienced members.";
            
        if (lowerQuestion.Contains("battery"))
            return "Car battery guidance:\n• Lifespan: Typically 3-5 years\n• Warning signs: Slow cranking, dim lights, dashboard warning\n• Testing: Free at most auto parts stores\n• Replacement: $100-200 for most vehicles\n\nCommunity members share battery testing tips, brand recommendations, and installation experiences in our electrical section.";
        
        // Default response for questions
        return "That's a great automotive question! Our community has extensive resources including:\n• Detailed guides and tutorials\n• Member experiences and reviews\n• Parts recommendations and pricing\n• Local service provider ratings\n• Interactive maps for finding services\n\nCould you be more specific about what you're looking for? I can direct you to the most relevant community resources and connect you with members who have similar experiences.";
    }

    private async Task<string> GenerateProblemSolutionAsync(string problem)
    {
        var lowerProblem = problem.ToLower();
        
        if (lowerProblem.Contains("won't start") || lowerProblem.Contains("not starting"))
            return "Car won't start? Here's a systematic approach:\n\n**Immediate checks:**\n• Battery connections (clean and tight)\n• Fuel level (ensure adequate fuel)\n• Listen for clicking sounds when turning key\n\n**If clicking sounds:** Likely battery issue - jump start or replacement needed\n**If no sound:** Could be starter, ignition, or fuel system\n\nOur community troubleshooting section has detailed diagnostic guides, and many members share their starting problem experiences. Would you like me to connect you with members who've solved similar issues?";
            
        if (lowerProblem.Contains("overheating") || lowerProblem.Contains("hot") || lowerProblem.Contains("temperature"))
            return "Overheating requires immediate attention:\n\n**Immediate actions:**\n1. Pull over safely and turn off engine\n2. Wait 30+ minutes for cooling\n3. Check coolant level when completely cool\n4. Look for visible leaks under the vehicle\n\n**Never remove radiator cap when hot!**\n\n**Common causes:** Low coolant, thermostat failure, water pump issues, radiator problems\n\nOur community has extensive overheating guides and member experiences with different solutions. Many members can recommend trusted cooling system specialists in your area.";
            
        if (lowerProblem.Contains("noise") || lowerProblem.Contains("sound") || lowerProblem.Contains("squealing") || 
            lowerProblem.Contains("grinding") || lowerProblem.Contains("clicking"))
            return "Strange noises can indicate various issues. Help me narrow it down:\n\n**Squealing:** Often belt-related or brake pads\n**Grinding:** Usually brakes or transmission\n**Clicking:** Could be CV joints or engine timing\n**Knocking:** Potentially serious engine issue\n\n**When does it occur?**\n• Starting up?\n• While driving?\n• When braking?\n• During turns?\n\nOur community diagnostic section has audio samples and member experiences with different automotive noises. I can connect you with members who've dealt with similar sounds.";
            
        if (lowerProblem.Contains("brake") || lowerProblem.Contains("brakes") || lowerProblem.Contains("stopping"))
            return "Brake problems require immediate professional attention for safety!\n\n**Warning signs:**\n• Squealing or grinding sounds\n• Soft or spongy pedal feel\n• Vehicle pulling to one side\n• Longer stopping distances\n• Brake warning light\n\n**Immediate action:** Have brakes inspected by a qualified technician\n\nOur community brake section has member experiences with brake repairs, trusted shop recommendations, and typical repair costs. Safety first - don't delay brake service!";
            
        if (lowerProblem.Contains("transmission") || lowerProblem.Contains("shifting") || lowerProblem.Contains("gear"))
            return "Transmission issues can be complex and costly:\n\n**Common symptoms:**\n• Hard or delayed shifting\n• Slipping gears\n• Unusual noises during shifting\n• Fluid leaks (red/brown fluid)\n• Burning smell\n\n**First steps:**\n• Check transmission fluid level and condition\n• Note when problems occur (cold start, highway, city driving)\n\nOur community transmission section has member experiences with repairs, rebuilds, and replacements. Many members can recommend trusted transmission specialists and share cost experiences.";
            
        if (lowerProblem.Contains("electrical") || lowerProblem.Contains("lights") || lowerProblem.Contains("battery") || 
            lowerProblem.Contains("alternator") || lowerProblem.Contains("charging"))
            return "Electrical problems can be tricky to diagnose:\n\n**Common issues:**\n• Battery not holding charge\n• Lights dimming while driving\n• Electrical accessories not working\n• Dashboard warning lights\n\n**Basic checks:**\n• Battery terminals (clean and tight)\n• Belt condition (alternator belt)\n• Fuse box for blown fuses\n\nOur community electrical section has diagnostic guides and member experiences with electrical repairs. Many members can help with troubleshooting steps and recommend qualified auto electricians.";
            
        return "I understand you're experiencing a vehicle issue. For safety and proper diagnosis, I recommend:\n\n1. **Document symptoms:** When, where, and how the problem occurs\n2. **Safety first:** Don't drive if it's unsafe\n3. **Community help:** Our troubleshooting section has guides for many common problems\n4. **Professional diagnosis:** Some issues require expert evaluation\n\nOur community has experienced members who've dealt with similar problems. Can you describe the specific symptoms you're experiencing? I can connect you with relevant community resources and member experiences.";
    }

    private async Task<string> GenerateGreetingResponseAsync(string userId)
    {
        var greetings = new[]
        {
            "Hello and welcome to the Community Car platform! 🚗 I'm your AI automotive assistant, ready to help with:\n\n• **Maintenance guidance** and service schedules\n• **Pricing information** and cost comparisons\n• **Local service recommendations** from community members\n• **Parts sourcing** and marketplace connections\n• **Technical support** and troubleshooting\n\nWhat automotive topic can I help you explore today?",
            
            "Hi there! Great to see you in our automotive community! 🔧 I'm here to assist with:\n\n• Finding **trusted mechanics** and service providers near you\n• **Maintenance tips** and scheduling guidance\n• **Parts recommendations** and pricing information\n• Connecting you with **community members** who share similar automotive interests\n• **DIY guides** and technical support\n\nHow can I help with your automotive needs today?",
            
            "Welcome to Community Car! 👋 I'm your dedicated automotive AI assistant. Our community offers:\n\n• **Expert advice** from experienced car enthusiasts\n• **Local service maps** with member reviews and ratings\n• **Marketplace** for buying/selling automotive parts\n• **Comprehensive guides** for maintenance and repairs\n• **Cost-saving tips** and budget-friendly solutions\n\nWhat would you like to know about cars, maintenance, or our community services?",
            
            "Hello! Welcome to the ultimate automotive community platform! 🚙 I'm here to help you with:\n\n• **Service scheduling** and maintenance planning\n• **Price comparisons** for parts and services\n• **Community recommendations** for trusted providers\n• **Technical troubleshooting** and repair guidance\n• **Local automotive maps** and service locators\n\nWhether you're a DIY enthusiast or prefer professional services, I can guide you to the right resources!"
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
        var lowerMessage = message.ToLower();
        
        // Community-specific responses
        if (lowerMessage.Contains("community") || lowerMessage.Contains("member") || lowerMessage.Contains("forum"))
        {
            return "Welcome to our automotive community! Here's what you can explore:\n• Share experiences and get advice from fellow car enthusiasts\n• Find trusted local mechanics and service providers\n• Buy/sell parts in our marketplace\n• Join discussions about specific car models\n• Access maintenance guides and tutorials\n\nWhat aspect of our community interests you most?";
        }
        
        // Pricing inquiries
        if (lowerMessage.Contains("price") || lowerMessage.Contains("cost") || lowerMessage.Contains("expensive") || lowerMessage.Contains("budget"))
        {
            return "Looking for automotive pricing information? Our community offers:\n• Real member experiences with service costs\n• Price comparisons from different providers\n• Group buying opportunities for parts\n• Budget-friendly maintenance tips\n• Cost-saving recommendations from experienced members\n\nWhat specific pricing information are you looking for?";
        }
        
        // Service-related inquiries
        if (lowerMessage.Contains("service") || lowerMessage.Contains("repair") || lowerMessage.Contains("fix") || lowerMessage.Contains("mechanic"))
        {
            return "Need automotive services? Our community can help:\n• Find highly-rated local mechanics and shops\n• Read detailed reviews from community members\n• Get service recommendations for your specific vehicle\n• Compare pricing and quality across providers\n• Access DIY guides for simple repairs\n\nWhat type of service or repair are you considering?";
        }
        
        // Parts-related inquiries
        if (lowerMessage.Contains("part") || lowerMessage.Contains("parts") || lowerMessage.Contains("buy") || lowerMessage.Contains("sell"))
        {
            return "Looking for automotive parts? Our community marketplace offers:\n• Quality parts from trusted community members\n• OEM and aftermarket options with reviews\n• Part number databases and compatibility guides\n• Group purchasing for better deals\n• Installation tips and tutorials\n\nWhat parts are you looking for, or do you have parts to sell?";
        }
        
        // Maps and location services
        if (lowerMessage.Contains("map") || lowerMessage.Contains("location") || lowerMessage.Contains("near") || lowerMessage.Contains("find"))
        {
            return "Our interactive maps can help you locate:\n• Trusted mechanics and auto shops with community ratings\n• Auto parts stores with current inventory and pricing\n• Gas stations with real-time fuel prices\n• Car washes and detailing services\n• Electric vehicle charging stations\n\nWhat type of automotive service are you trying to locate?";
        }
        
        // Maintenance inquiries
        if (lowerMessage.Contains("maintenance") || lowerMessage.Contains("care") || lowerMessage.Contains("keep") || lowerMessage.Contains("maintain"))
        {
            return "Vehicle maintenance is key to longevity! Our community provides:\n• Comprehensive maintenance schedules and checklists\n• Member experiences with different service intervals\n• DIY maintenance guides with step-by-step instructions\n• Seasonal maintenance tips and reminders\n• Cost-effective maintenance strategies\n\nWhat maintenance topics are you interested in learning about?";
        }
        
        // General automotive topics
        if (lowerMessage.Contains("car") || lowerMessage.Contains("vehicle") || lowerMessage.Contains("auto"))
        {
            return "Great to see your interest in automotive topics! Our community covers:\n• Vehicle maintenance and repair guidance\n• Parts sourcing and installation help\n• Local service provider recommendations\n• Buying and selling advice\n• Technical discussions and troubleshooting\n\nWhat specific automotive area would you like to explore?";
        }
        
        // Default community-focused response
        return "Welcome to our automotive community! I'm here to help you with:\n\n🔧 **Maintenance & Repairs**: Schedules, guides, and member experiences\n💰 **Pricing & Costs**: Real pricing data and budget-friendly options\n🗺️ **Local Services**: Find trusted mechanics, parts stores, and services near you\n🛒 **Parts & Marketplace**: Buy/sell parts with community members\n👥 **Community Support**: Connect with experienced car enthusiasts\n\nWhat automotive topic can I help you with today? Feel free to ask about specific services, parts, pricing, or maintenance needs!";
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

    private List<string> ExtractKeywords(string text)
    {
        return text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 3)
            .Select(w => w.Trim('.', ',', '!', '?', ';', ':').ToLowerInvariant())
            .Where(w => !string.IsNullOrEmpty(w))
            .Distinct()
            .Take(5)
            .ToList();
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