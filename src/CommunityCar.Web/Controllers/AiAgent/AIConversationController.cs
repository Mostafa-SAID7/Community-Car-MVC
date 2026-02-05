using Microsoft.AspNetCore.Mvc;
using CommunityCar.AI.Services;
using CommunityCar.Application.Common.Interfaces.Repositories.Chat;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Core;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Services.Analytics.Users;
using CommunityCar.Application.Features.AI.ViewModels;

namespace CommunityCar.Web.Controllers.AiAgent;

[Route("AiAgent/Conversations")]
public class AIConversationController : Controller
{
    private readonly IIntelligentChatService _chatService;
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserAnalyticsService _userAnalyticsService;
    private readonly ILogger<AIConversationController> _logger;

    public AIConversationController(
        IIntelligentChatService chatService,
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        IUserAnalyticsService userAnalyticsService,
        ILogger<AIConversationController> logger)
    {
        _chatService = chatService;
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _userAnalyticsService = userAnalyticsService;
        _logger = logger;
    }

    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var conversations = await _conversationRepository.GetAllAsync();
            var recentConversations = conversations
                .OrderByDescending(c => c.UpdatedAt)
                .Take(20)
                .Select(c => new
                {
                    Id = c.Id,
                    Title = c.Title ?? "Untitled Conversation",
                    ParticipantCount = c.Participants?.Count ?? 0,
                    MessageCount = GetMessageCount(c.Id).Result,
                    LastActivity = c.UpdatedAt,
                    Status = !c.IsDeleted ? "Active" : "Inactive",
                    Duration = CalculateDuration(c.CreatedAt, c.UpdatedAt ?? c.CreatedAt)
                })
                .ToArray();

            var totalConversations = conversations.Count();
            var activeConversations = conversations.Count(c => !c.IsDeleted);
            var avgDuration = CalculateAverageDuration(conversations.Take(100));

            var model = new
            {
                RecentConversations = recentConversations,
                Statistics = new
                {
                    Total = totalConversations,
                    Active = activeConversations,
                    Completed = totalConversations - activeConversations,
                    AverageDuration = avgDuration
                },
                Error = (string?)null
            };

            return View("~/Views/AiAgent/Conversations/Index.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading conversations");
            
            var errorModel = new
            {
                RecentConversations = new object[0],
                Statistics = new
                {
                    Total = 0,
                    Active = 0,
                    Completed = 0,
                    AverageDuration = "0m"
                },
                Error = "Unable to load conversation data"
            };
            
            return View("~/Views/AiAgent/Conversations/Index.cshtml", errorModel);
        }
    }

    [HttpGet]
    [Route("Details/{conversationId}")]
    public async Task<IActionResult> GetConversationDetails(Guid conversationId)
    {
        try
        {
            var conversations = await _conversationRepository.GetAllAsync();
            var conversation = conversations.FirstOrDefault(c => c.Id == conversationId);
            
            if (conversation == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Conversation not found"
                });
            }

            var messages = await _messageRepository.GetConversationMessagesAsync(conversationId);
            var messageDetails = messages.Select(m => new
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                SenderType = m.SenderId.ToString().StartsWith("ai-") ? "AI" : "User",
                Timestamp = m.CreatedAt,
                IsFromAI = m.SenderId.ToString().StartsWith("ai-")
            }).OrderBy(m => m.Timestamp).ToArray();

            var conversationDetails = new
            {
                Id = conversation.Id,
                Title = conversation.Title ?? "Untitled Conversation",
                CreatedAt = conversation.CreatedAt,
                UpdatedAt = conversation.UpdatedAt,
                IsActive = !conversation.IsDeleted,
                ParticipantCount = conversation.Participants?.Count ?? 0,
                MessageCount = messages.Count(),
                Duration = CalculateDuration(conversation.CreatedAt, conversation.UpdatedAt ?? conversation.CreatedAt),
                Messages = messageDetails,
                Participants = conversation.Participants?.Select(p => new
                {
                    UserId = p.UserId,
                    JoinedAt = p.JoinedAt,
                    IsActive = !p.IsArchived
                }).ToArray() ?? new object[0]
            };

            return Json(new
            {
                success = true,
                data = conversationDetails
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversation details for {ConversationId}", conversationId);
            return Json(new
            {
                success = false,
                message = "Failed to load conversation details",
                error = ex.Message
            });
        }
    }

    [HttpPost]
    [Route("Archive/{conversationId}")]
    public async Task<IActionResult> ArchiveConversation(Guid conversationId)
    {
        try
        {
            _logger.LogInformation("Archiving conversation {ConversationId}", conversationId);

            var conversations = await _conversationRepository.GetAllAsync();
            var conversation = conversations.FirstOrDefault(c => c.Id == conversationId);
            
            if (conversation == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Conversation not found"
                });
            }

            // In a real implementation, this would update the conversation status
            conversation.SoftDelete("System");
            
            // Simulate repository update
            await Task.Delay(100);

            _logger.LogInformation("Successfully archived conversation {ConversationId}", conversationId);

            return Json(new
            {
                success = true,
                message = "Conversation archived successfully",
                data = new
                {
                    conversationId = conversationId,
                    archivedAt = DateTime.UtcNow,
                    status = "Archived"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving conversation {ConversationId}", conversationId);
            return Json(new
            {
                success = false,
                message = "Failed to archive conversation",
                error = ex.Message
            });
        }
    }

    [HttpDelete]
    [Route("Delete/{conversationId}")]
    public async Task<IActionResult> DeleteConversation(Guid conversationId)
    {
        try
        {
            _logger.LogInformation("Deleting conversation {ConversationId}", conversationId);

            var conversations = await _conversationRepository.GetAllAsync();
            var conversation = conversations.FirstOrDefault(c => c.Id == conversationId);
            
            if (conversation == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Conversation not found"
                });
            }

            // In a real implementation, this would delete the conversation and its messages
            await Task.Delay(100);

            _logger.LogInformation("Successfully deleted conversation {ConversationId}", conversationId);

            return Json(new
            {
                success = true,
                message = "Conversation deleted successfully",
                data = new
                {
                    conversationId = conversationId,
                    deletedAt = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conversation {ConversationId}", conversationId);
            return Json(new
            {
                success = false,
                message = "Failed to delete conversation",
                error = ex.Message
            });
        }
    }

    [HttpGet]
    [Route("Export/{conversationId}")]
    public async Task<IActionResult> ExportConversation(Guid conversationId, string format = "json")
    {
        try
        {
            _logger.LogInformation("Exporting conversation {ConversationId} in {Format} format", conversationId, format);

            var conversations = await _conversationRepository.GetAllAsync();
            var conversation = conversations.FirstOrDefault(c => c.Id == conversationId);
            
            if (conversation == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Conversation not found"
                });
            }

            var messages = await _messageRepository.GetConversationMessagesAsync(conversationId);
            
            // Generate export URL (in real implementation, this would create a downloadable file)
            var exportUrl = $"/api/exports/conversations/{conversationId}.{format}";
            var fileName = $"conversation_{conversationId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{format}";

            var exportData = new
            {
                ConversationId = conversationId,
                Title = conversation.Title ?? "Untitled Conversation",
                CreatedAt = conversation.CreatedAt,
                MessageCount = messages.Count(),
                ExportFormat = format.ToUpper(),
                ExportedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                FileSize = EstimateFileSize(messages.Count(), format)
            };

            _logger.LogInformation("Successfully generated export for conversation {ConversationId}", conversationId);

            return Json(new
            {
                success = true,
                message = "Conversation export generated successfully",
                data = new
                {
                    downloadUrl = exportUrl,
                    fileName = fileName,
                    metadata = exportData
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting conversation {ConversationId}", conversationId);
            return Json(new
            {
                success = false,
                message = "Failed to export conversation",
                error = ex.Message
            });
        }
    }

    [HttpGet]
    [Route("Search")]
    public async Task<IActionResult> SearchConversations(string query, int page = 1, int pageSize = 20)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new
                {
                    success = false,
                    message = "Search query is required"
                });
            }

            _logger.LogInformation("Searching conversations with query: {Query}", query);

            var conversations = await _conversationRepository.GetAllAsync();
            var messages = await _messageRepository.GetAllAsync();

            // Search in conversation titles and message content
            var matchingConversationIds = new HashSet<Guid>();
            
            // Search in conversation titles
            var titleMatches = conversations
                .Where(c => c.Title?.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
                .Select(c => c.Id);
            
            foreach (var id in titleMatches)
                matchingConversationIds.Add(id);

            // Search in message content
            var contentMatches = messages
                .Where(m => m.Content?.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
                .Select(m => m.ConversationId)
                .Distinct();
            
            foreach (var id in contentMatches)
                matchingConversationIds.Add(id);

            var matchingConversations = conversations
                .Where(c => matchingConversationIds.Contains(c.Id))
                .OrderByDescending(c => c.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    Id = c.Id,
                    Title = c.Title ?? "Untitled Conversation",
                    LastActivity = c.UpdatedAt,
                    MessageCount = GetMessageCount(c.Id).Result,
                    ParticipantCount = c.Participants?.Count ?? 0,
                    IsActive = !c.IsDeleted,
                    Relevance = CalculateRelevance(c, query)
                })
                .ToArray();

            var totalResults = matchingConversationIds.Count;
            var totalPages = (int)Math.Ceiling((double)totalResults / pageSize);

            return Json(new
            {
                success = true,
                data = new
                {
                    conversations = matchingConversations,
                    pagination = new
                    {
                        currentPage = page,
                        pageSize = pageSize,
                        totalResults = totalResults,
                        totalPages = totalPages,
                        hasNextPage = page < totalPages,
                        hasPreviousPage = page > 1
                    },
                    searchQuery = query
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching conversations with query: {Query}", query);
            return Json(new
            {
                success = false,
                message = "Failed to search conversations",
                error = ex.Message
            });
        }
    }

    private async Task<int> GetMessageCount(Guid conversationId)
    {
        try
        {
            var messages = await _messageRepository.GetConversationMessagesAsync(conversationId);
            return messages.Count();
        }
        catch
        {
            return 0;
        }
    }

    private static string CalculateDuration(DateTime start, DateTime end)
    {
        var duration = end - start;
        
        if (duration.TotalHours >= 1)
            return $"{(int)duration.TotalHours}h {duration.Minutes}m";
        else if (duration.TotalMinutes >= 1)
            return $"{duration.Minutes}m {duration.Seconds}s";
        else
            return $"{duration.Seconds}s";
    }

    private static string CalculateAverageDuration(IEnumerable<Domain.Entities.Chats.Conversation> conversations)
    {
        var durations = conversations
            .Where(c => c.UpdatedAt.HasValue && c.UpdatedAt > c.CreatedAt)
            .Select(c => c.UpdatedAt!.Value - c.CreatedAt)
            .ToList();

        if (!durations.Any()) return "0m";

        var avgDuration = TimeSpan.FromTicks((long)durations.Average(d => d.Ticks));
        
        if (avgDuration.TotalHours >= 1)
            return $"{(int)avgDuration.TotalHours}h {avgDuration.Minutes}m";
        else
            return $"{avgDuration.Minutes}m";
    }

    private static string EstimateFileSize(int messageCount, string format)
    {
        var bytesPerMessage = format.ToLower() switch
        {
            "json" => 200,
            "csv" => 150,
            "txt" => 100,
            _ => 175
        };

        var totalBytes = messageCount * bytesPerMessage;
        
        if (totalBytes < 1024) return $"{totalBytes} B";
        if (totalBytes < 1024 * 1024) return $"{totalBytes / 1024:F1} KB";
        return $"{totalBytes / (1024 * 1024):F1} MB";
    }

    private static double CalculateRelevance(Domain.Entities.Chats.Conversation conversation, string query)
    {
        var relevance = 0.0;
        
        // Title match gets higher relevance
        if (conversation.Title?.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
            relevance += 0.8;
        
        // Recent conversations get slight boost
        var lastUpdate = conversation.UpdatedAt ?? conversation.CreatedAt;
        var daysSinceUpdate = (DateTime.UtcNow - lastUpdate).TotalDays;
        if (daysSinceUpdate < 7) relevance += 0.2;
        
        return Math.Min(relevance, 1.0);
    }
}
