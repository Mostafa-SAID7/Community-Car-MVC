using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.AI.Services;
using CommunityCar.AI.Models;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Web.Controllers.Api;

[Route("api/ai/chat")]
[ApiController]
[Authorize]
public class AIChatAssistantController : ControllerBase
{
    private readonly IIntelligentChatService _chatService;
    private readonly ISentimentAnalysisService _sentimentService;
    private readonly IPredictionService _predictionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AIChatAssistantController> _logger;

    public AIChatAssistantController(
        IIntelligentChatService chatService,
        ISentimentAnalysisService sentimentService,
        IPredictionService predictionService,
        ICurrentUserService currentUserService,
        ILogger<AIChatAssistantController> logger)
    {
        _chatService = chatService;
        _sentimentService = sentimentService;
        _predictionService = predictionService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Send a message to the AI chat assistant
    /// </summary>
    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { success = false, message = "Message cannot be empty" });

            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            // Use provided conversation ID or create new one
            var conversationId = request.ConversationId ?? Guid.NewGuid();

            // Get AI response
            var response = await _chatService.GetResponseAsync(request.Message, userId, request.Context);
            
            // Set the conversation ID in response
            response.ConversationId = conversationId;

            // Analyze sentiment of the user's message
            var sentiment = await _sentimentService.AnalyzeSentimentAsync(request.Message);

            return Ok(new
            {
                success = true,
                data = new
                {
                    response = response.Message,
                    confidence = response.Confidence,
                    suggestions = response.Suggestions,
                    sentiment = new
                    {
                        label = sentiment.Label,
                        score = sentiment.Score,
                        confidence = sentiment.Confidence,
                        emotions = sentiment.Emotions
                    },
                    conversationId = response.ConversationId,
                    timestamp = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing AI chat message for user {UserId}", _currentUserService.UserId);
            return BadRequest(new { success = false, message = "Failed to process message" });
        }
    }

    /// <summary>
    /// Get conversation history
    /// </summary>
    [HttpGet("conversation/{conversationId}")]
    public async Task<IActionResult> GetConversation(Guid conversationId)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            var conversation = await _chatService.GetConversationHistoryAsync(conversationId, userId);
            
            if (conversation == null)
                return NotFound(new { success = false, message = "Conversation not found" });

            return Ok(new { success = true, data = conversation });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversation {ConversationId}", conversationId);
            return BadRequest(new { success = false, message = "Failed to retrieve conversation" });
        }
    }

    /// <summary>
    /// Get user's conversation list
    /// </summary>
    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            var conversations = await _chatService.GetUserConversationsAsync(userId, page, pageSize);
            return Ok(new { success = true, data = conversations });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversations for user {UserId}", _currentUserService.UserId);
            return BadRequest(new { success = false, message = "Failed to retrieve conversations" });
        }
    }

    /// <summary>
    /// Start a new conversation
    /// </summary>
    [HttpPost("conversation")]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            var conversation = await _chatService.StartConversationAsync(userId, request.Title, request.Context);
            return Ok(new { success = true, data = conversation });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting conversation for user {UserId}", _currentUserService.UserId);
            return BadRequest(new { success = false, message = "Failed to start conversation" });
        }
    }

    /// <summary>
    /// Delete a conversation
    /// </summary>
    [HttpDelete("conversation/{conversationId}")]
    public async Task<IActionResult> DeleteConversation(Guid conversationId)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            var result = await _chatService.DeleteConversationAsync(conversationId, userId);
            
            if (!result)
                return NotFound(new { success = false, message = "Conversation not found or access denied" });

            return Ok(new { success = true, message = "Conversation deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conversation {ConversationId}", conversationId);
            return BadRequest(new { success = false, message = "Failed to delete conversation" });
        }
    }

    /// <summary>
    /// Get AI suggestions based on context
    /// </summary>
    [HttpPost("suggestions")]
    public async Task<IActionResult> GetSuggestions([FromBody] SuggestionRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            var suggestions = await _chatService.GetSuggestionsAsync(request.Context, userId, request.MaxSuggestions);
            return Ok(new { success = true, data = suggestions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting AI suggestions for user {UserId}", _currentUserService.UserId);
            return BadRequest(new { success = false, message = "Failed to get suggestions" });
        }
    }

    /// <summary>
    /// Analyze sentiment of text
    /// </summary>
    [HttpPost("sentiment")]
    public async Task<IActionResult> AnalyzeSentiment([FromBody] SentimentRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { success = false, message = "Text cannot be empty" });

            var sentiment = await _sentimentService.AnalyzeSentimentAsync(request.Text);
            
            return Ok(new
            {
                success = true,
                data = new
                {
                    text = request.Text,
                    sentiment = sentiment.Label,
                    score = sentiment.Score,
                    confidence = sentiment.Confidence,
                    emotions = sentiment.Emotions
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing sentiment");
            return BadRequest(new { success = false, message = "Failed to analyze sentiment" });
        }
    }

    /// <summary>
    /// Get AI predictions for user behavior
    /// </summary>
    [HttpGet("predictions")]
    public async Task<IActionResult> GetPredictions([FromQuery] string type = "engagement")
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            var predictions = type.ToLower() switch
            {
                "engagement" => await _predictionService.PredictUserEngagementAsync(userId),
                "interests" => await _predictionService.PredictUserInterestsAsync(userId),
                "content" => await _predictionService.PredictContentPreferencesAsync(userId),
                _ => await _predictionService.GetUserPredictionsAsync(userId)
            };

            return Ok(new { success = true, data = predictions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting predictions for user {UserId}", _currentUserService.UserId);
            return BadRequest(new { success = false, message = "Failed to get predictions" });
        }
    }

    /// <summary>
    /// Get AI-powered content recommendations
    /// </summary>
    [HttpGet("recommendations")]
    public async Task<IActionResult> GetRecommendations([FromQuery] string? category = null, [FromQuery] int count = 10)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            var recommendations = await _predictionService.GetContentRecommendationsAsync(userId, category, count);
            return Ok(new { success = true, data = recommendations });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommendations for user {UserId}", _currentUserService.UserId);
            return BadRequest(new { success = false, message = "Failed to get recommendations" });
        }
    }

    /// <summary>
    /// Train AI model with user feedback
    /// </summary>
    [HttpPost("feedback")]
    public async Task<IActionResult> ProvideFeedback([FromBody] FeedbackRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            await _chatService.ProcessFeedbackAsync(request.ConversationId, request.MessageId, request.Rating, request.Feedback, userId);
            return Ok(new { success = true, message = "Feedback processed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing feedback for user {UserId}", _currentUserService.UserId);
            return BadRequest(new { success = false, message = "Failed to process feedback" });
        }
    }

    /// <summary>
    /// Get AI chat statistics
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetChatStats()
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            var stats = await _chatService.GetUserChatStatsAsync(userId);
            return Ok(new { success = true, data = stats });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chat stats for user {UserId}", _currentUserService.UserId);
            return BadRequest(new { success = false, message = "Failed to get chat statistics" });
        }
    }

    /// <summary>
    /// Get AI assistant capabilities and features
    /// </summary>
    [HttpGet("capabilities")]
    public IActionResult GetCapabilities()
    {
        try
        {
            var capabilities = new
            {
                features = new[]
                {
                    "Natural language conversation",
                    "Car-related expertise",
                    "Sentiment analysis",
                    "Content recommendations",
                    "User behavior predictions",
                    "Multi-language support",
                    "Context-aware responses"
                },
                supportedLanguages = new[] { "en", "ar" },
                maxMessageLength = 2000,
                conversationRetention = "30 days",
                version = "1.0.0"
            };

            return Ok(new { success = true, data = capabilities });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting AI capabilities");
            return BadRequest(new { success = false, message = "Failed to get capabilities" });
        }
    }
}

// Request/Response models
public class ChatMessageRequest
{
    public string Message { get; set; } = string.Empty;
    public string? Context { get; set; }
    public Guid? ConversationId { get; set; }
}

public class StartConversationRequest
{
    public string? Title { get; set; }
    public string? Context { get; set; }
}

public class SuggestionRequest
{
    public string Context { get; set; } = string.Empty;
    public int MaxSuggestions { get; set; } = 5;
}

public class SentimentRequest
{
    public string Text { get; set; } = string.Empty;
}

public class FeedbackRequest
{
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
    public int Rating { get; set; } // 1-5 stars
    public string? Feedback { get; set; }
}
 