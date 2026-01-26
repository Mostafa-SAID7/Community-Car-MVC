using CommunityCar.AI.Services;
using CommunityCar.AI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityCar.Web.Controllers.Api;

[ApiController]
[Route("api/ai/chat")]
[IgnoreAntiforgeryToken]
public class AIChatAssistantController : ControllerBase
{
    private readonly IIntelligentChatService _intelligentChatService;
    private readonly ILogger<AIChatAssistantController> _logger;

    public AIChatAssistantController(
        IIntelligentChatService intelligentChatService,
        ILogger<AIChatAssistantController> logger)
    {
        _intelligentChatService = intelligentChatService;
        _logger = logger;
    }

    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return Ok(new
                {
                    success = false,
                    message = "Message cannot be empty."
                });
            }

            var userId = GetCurrentUserId();
            _logger.LogInformation("Processing chat message for user {UserId}: {Message}", userId, request.Message);
            
            var response = await _intelligentChatService.GetResponseAsync(request.Message, userId, request.Context);
            
            return Ok(new
            {
                success = true,
                data = new
                {
                    response = response.Message,
                    messageId = response.MessageId,
                    confidence = response.Confidence,
                    suggestions = response.Suggestions,
                    conversationId = response.ConversationId,
                    sentimentAnalysis = response.SentimentAnalysis,
                    engagementScore = response.EngagementScore,
                    extractedTopics = response.ExtractedTopics,
                    timestamp = response.Timestamp
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message: {Message}", ex.Message);
            return Ok(new
            {
                success = false,
                message = "I'm sorry, I encountered an error processing your message. Please try again.",
                error = ex.Message // Include error details for debugging
            });
        }
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            var conversations = await _intelligentChatService.GetUserConversationsAsync(userId, page, pageSize);
            
            return Ok(new
            {
                success = true,
                data = conversations
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversations for user {UserId}", GetCurrentUserId());
            return Ok(new
            {
                success = false,
                message = "Failed to retrieve conversations"
            });
        }
    }

    [HttpGet("conversations/{conversationId}")]
    public async Task<IActionResult> GetConversationHistory(Guid conversationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var conversation = await _intelligentChatService.GetConversationHistoryAsync(conversationId, userId);
            
            if (conversation == null)
            {
                return Ok(new
                {
                    success = false,
                    message = "Conversation not found"
                });
            }
            
            return Ok(new
            {
                success = true,
                data = conversation
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversation {ConversationId} for user {UserId}", conversationId, GetCurrentUserId());
            return Ok(new
            {
                success = false,
                message = "Failed to retrieve conversation"
            });
        }
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var conversation = await _intelligentChatService.StartConversationAsync(userId, request.Title, request.Context);
            
            return Ok(new
            {
                success = true,
                data = conversation
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting conversation for user {UserId}", GetCurrentUserId());
            return Ok(new
            {
                success = false,
                message = "Failed to start conversation"
            });
        }
    }

    [HttpDelete("conversations/{conversationId}")]
    public async Task<IActionResult> DeleteConversation(Guid conversationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _intelligentChatService.DeleteConversationAsync(conversationId, userId);
            
            return Ok(new
            {
                success = success,
                message = success ? "Conversation deleted successfully" : "Conversation not found or cannot be deleted"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conversation {ConversationId} for user {UserId}", conversationId, GetCurrentUserId());
            return Ok(new
            {
                success = false,
                message = "Failed to delete conversation"
            });
        }
    }

    [HttpGet("suggestions")]
    public async Task<IActionResult> GetSuggestions([FromQuery] string context = "", [FromQuery] int maxSuggestions = 5)
    {
        try
        {
            var userId = GetCurrentUserId();
            var suggestions = await _intelligentChatService.GetSuggestionsAsync(context, userId, maxSuggestions);
            
            return Ok(new
            {
                success = true,
                data = new { suggestions }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting suggestions for user {UserId}", GetCurrentUserId());
            return Ok(new
            {
                success = false,
                message = "Failed to get suggestions"
            });
        }
    }

    [HttpPost("feedback")]
    public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _intelligentChatService.ProcessFeedbackAsync(
                request.ConversationId, 
                request.MessageId, 
                request.Rating, 
                request.Feedback, 
                userId);
            
            return Ok(new
            {
                success = true,
                message = "Feedback submitted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for user {UserId}", GetCurrentUserId());
            return Ok(new
            {
                success = false,
                message = "Failed to submit feedback"
            });
        }
    }

    [HttpGet("test")]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            _logger.LogInformation("AI Chat test endpoint called");
            
            return Ok(new
            {
                success = true,
                message = "AI Chat service is working",
                timestamp = DateTime.UtcNow,
                userId = GetCurrentUserId()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in test endpoint: {Message}", ex.Message);
            return Ok(new
            {
                success = false,
                message = "Test failed",
                error = ex.Message
            });
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetUserStats()
    {
        try
        {
            var userId = GetCurrentUserId();
            var stats = await _intelligentChatService.GetUserChatStatsAsync(userId);
            
            return Ok(new
            {
                success = true,
                data = stats
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stats for user {UserId}", GetCurrentUserId());
            return Ok(new
            {
                success = false,
                message = "Failed to get user stats"
            });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        
        // Fallback for development/testing - use a consistent test user ID
        // In production, this should be replaced with proper authentication
        return new Guid("12345678-1234-1234-1234-123456789012");
    }
}

// Request/Response models
public class ChatMessageRequest
{
    public string Message { get; set; } = string.Empty;
    public string? Context { get; set; }
    public string? ConversationId { get; set; }
}

public class StartConversationRequest
{
    public string? Title { get; set; }
    public string? Context { get; set; }
}

public class FeedbackRequest
{
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
    public int Rating { get; set; }
    public string? Feedback { get; set; }
}