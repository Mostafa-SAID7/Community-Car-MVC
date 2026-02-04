using CommunityCar.AI.Services;
using CommunityCar.AI.Models;
using CommunityCar.Application.Features.Communication.Chat.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CommunityCar.Web.Controllers.AiAgent;

[Controller]
[Route("ai-chat")]
[Authorize]
public class AIChatController : Controller
{
    private readonly IIntelligentChatService _intelligentChatService;
    private readonly ILogger<AIChatController> _logger;

    public AIChatController(
        IIntelligentChatService intelligentChatService,
        ILogger<AIChatController> logger)
    {
        _intelligentChatService = intelligentChatService;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequestVM request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message cannot be empty.");
        }

        try
        {
            var userId = GetCurrentUserId();
            var response = await _intelligentChatService.GetResponseAsync(request.Message, userId, request.Context);
            
            return Ok(new
            {
                messageId = response.MessageId,
                message = response.Message,
                confidence = response.Confidence,
                suggestions = response.Suggestions,
                conversationId = response.ConversationId,
                sentimentAnalysis = response.SentimentAnalysis,
                engagementScore = response.EngagementScore,
                extractedTopics = response.ExtractedTopics,
                timestamp = response.Timestamp
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { error = "AI service error", details = ex.Message });
        }
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            var conversations = await _intelligentChatService.GetUserConversationsAsync(userId, page, pageSize);
            return Ok(conversations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversations for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { error = "Failed to retrieve conversations" });
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
                return NotFound("Conversation not found");
            }
            
            return Ok(conversation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversation {ConversationId} for user {UserId}", conversationId, GetCurrentUserId());
            return StatusCode(500, new { error = "Failed to retrieve conversation" });
        }
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequestVM request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var conversation = await _intelligentChatService.StartConversationAsync(userId, request.Title, request.Context);
            return Ok(conversation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting conversation for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { error = "Failed to start conversation" });
        }
    }

    [HttpDelete("conversations/{conversationId}")]
    public async Task<IActionResult> DeleteConversation(Guid conversationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _intelligentChatService.DeleteConversationAsync(conversationId, userId);
            
            if (!success)
            {
                return NotFound("Conversation not found or cannot be deleted");
            }
            
            return Ok(new { message = "Conversation deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conversation {ConversationId} for user {UserId}", conversationId, GetCurrentUserId());
            return StatusCode(500, new { error = "Failed to delete conversation" });
        }
    }

    [HttpGet("suggestions")]
    public async Task<IActionResult> GetSuggestions([FromQuery] string context = "", [FromQuery] int maxSuggestions = 5)
    {
        try
        {
            var userId = GetCurrentUserId();
            var suggestions = await _intelligentChatService.GetSuggestionsAsync(context, userId, maxSuggestions);
            return Ok(new { suggestions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting suggestions for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { error = "Failed to get suggestions" });
        }
    }

    [HttpPost("feedback")]
    public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackRequestVM request)
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
            
            return Ok(new { message = "Feedback submitted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { error = "Failed to submit feedback" });
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetUserStats()
    {
        try
        {
            var userId = GetCurrentUserId();
            var stats = await _intelligentChatService.GetUserChatStatsAsync(userId);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stats for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { error = "Failed to get user stats" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        
        // Fallback for development/testing
        return Guid.NewGuid();
    }
}



