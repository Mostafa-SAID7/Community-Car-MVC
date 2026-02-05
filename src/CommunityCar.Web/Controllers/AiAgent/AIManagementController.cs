using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Services.AI;
using CommunityCar.Application.Common.Interfaces.Services.Account.Core;
using CommunityCar.Application.Common.Interfaces.Repositories.Chat;

namespace CommunityCar.Web.Controllers.AiAgent;

[Route("ai-agent")]
public class AIManagementController : Controller
{
    private readonly IAIManagementService _aiManagementService;
    private readonly IUserAnalyticsService _userAnalyticsService;
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<AIManagementController> _logger;

    public AIManagementController(
        IAIManagementService aiManagementService,
        IUserAnalyticsService userAnalyticsService,
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        ILogger<AIManagementController> logger)
    {
        _aiManagementService = aiManagementService;
        _userAnalyticsService = userAnalyticsService;
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _logger = logger;
    }

    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            // Get real data from AI management service
            var models = await _aiManagementService.GetAllModelsAsync();
            var trainingQueue = await _aiManagementService.GetTrainingQueueAsync();
            var recentTraining = await _aiManagementService.GetRecentTrainingHistoryAsync(3);

            // Get real conversation data
            var totalConversations = await GetTotalConversationsAsync();
            var activeUsers = await _userAnalyticsService.GetActiveUsersCountAsync();
            var todayMessages = await GetTodayMessagesCountAsync();
            var weeklyGrowth = await GetWeeklyGrowthAsync();
            var recentActivity = await GetRecentActivityAsync();
            
            var model = new
            {
                TotalConversations = totalConversations,
                ActiveUsers = activeUsers,
                ResponseTime = await CalculateAverageResponseTimeAsync(),
                SatisfactionRate = await CalculateSatisfactionRateAsync(),
                TodayMessages = todayMessages,
                WeeklyGrowth = weeklyGrowth,
                TopIntents = await GetTopIntentsAsync(),
                RecentActivity = recentActivity,
                ActiveModels = models.Count(m => m.IsActive),
                TrainingJobs = trainingQueue.Count(),
                RecentTrainingCount = recentTraining.Count()
            };

            return View("~/Views/AiAgent/Index.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading AI Management dashboard");
            return View("~/Views/AiAgent/Index.cshtml", new { Error = "Unable to load dashboard data" });
        }
    }

    private async Task<int> GetTotalConversationsAsync()
    {
        try
        {
            var conversations = await _conversationRepository.GetAllAsync();
            return conversations.Count();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total conversations count");
            return 0;
        }
    }

    private async Task<int> GetTodayMessagesCountAsync()
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var messages = await _messageRepository.GetAllAsync();
            return messages.Count(m => m.CreatedAt.Date == today);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's messages count");
            return 0;
        }
    }

    private async Task<double> GetWeeklyGrowthAsync()
    {
        try
        {
            var thisWeek = DateTime.UtcNow.AddDays(-7);
            var lastWeek = DateTime.UtcNow.AddDays(-14);
            
            var conversations = await _conversationRepository.GetAllAsync();
            var thisWeekCount = conversations.Count(c => c.CreatedAt >= thisWeek);
            var lastWeekCount = conversations.Count(c => c.CreatedAt >= lastWeek && c.CreatedAt < thisWeek);
            
            if (lastWeekCount == 0) return thisWeekCount > 0 ? 100.0 : 0.0;
            
            return ((double)(thisWeekCount - lastWeekCount) / lastWeekCount) * 100.0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating weekly growth");
            return 0.0;
        }
    }

    private async Task<object[]> GetRecentActivityAsync()
    {
        try
        {
            var recentMessages = await _messageRepository.GetAllAsync();
            var recent = recentMessages
                .OrderByDescending(m => m.CreatedAt)
                .Take(3)
                .Select(m => new
                {
                    Time = GetTimeAgo(m.CreatedAt),
                    User = $"User{m.SenderId.ToString()[..8]}", // Anonymized user ID
                    Message = TruncateMessage(m.Content)
                })
                .ToArray();

            return recent.Length > 0 ? recent : new[]
            {
                new { Time = "No recent activity", User = "", Message = "" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent activity");
            return new[]
            {
                new { Time = "Error loading", User = "", Message = "Unable to load recent activity" }
            };
        }
    }

    private async Task<string> CalculateAverageResponseTimeAsync()
    {
        try
        {
            // This would need to be tracked in real implementation
            // For now, calculate based on AI service performance metrics
            var models = await _aiManagementService.GetAllModelsAsync();
            var activeModels = models.Where(m => m.IsActive).ToList();
            
            if (!activeModels.Any()) return "N/A";
            
            // Simulate response time calculation
            var avgResponseTime = activeModels.Average(m => 1200); // Default 1.2s
            return $"{avgResponseTime / 1000:F1}s";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating average response time");
            return "N/A";
        }
    }

    private async Task<double> CalculateSatisfactionRateAsync()
    {
        try
        {
            // This would be based on user feedback/ratings in real implementation
            // For now, estimate based on conversation completion rates
            var conversations = await _conversationRepository.GetAllAsync();
            if (!conversations.Any()) return 0.0;

            var completedConversations = 0;
            foreach (var conversation in conversations.Take(100)) // Sample for performance
            {
                var messages = await _messageRepository.GetConversationMessagesAsync(conversation.Id);
                if (messages.Count() >= 3) // Consider conversations with 3+ messages as satisfactory
                {
                    completedConversations++;
                }
            }

            return Math.Min(conversations.Count(), 100) > 0 ? 
                (completedConversations * 100.0 / Math.Min(conversations.Count(), 100)) : 0.0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating satisfaction rate");
            return 0.0;
        }
    }

    private async Task<string[]> GetTopIntentsAsync()
    {
        try
        {
            // This would be based on intent classification in real implementation
            // For now, return common car-related intents
            var messages = await _messageRepository.GetAllAsync();
            var recentMessages = messages
                .OrderByDescending(m => m.CreatedAt)
                .Take(1000)
                .Select(m => m.Content?.ToLower() ?? "")
                .Where(content => !string.IsNullOrEmpty(content))
                .ToList();

            var intentKeywords = new Dictionary<string, string[]>
            {
                ["Car Maintenance"] = new[] { "maintenance", "service", "oil", "repair", "check" },
                ["Insurance"] = new[] { "insurance", "coverage", "claim", "policy", "premium" },
                ["Troubleshooting"] = new[] { "problem", "issue", "error", "trouble", "broken" },
                ["Recommendations"] = new[] { "recommend", "suggest", "advice", "best", "should" },
                ["Purchase"] = new[] { "buy", "purchase", "price", "cost", "deal" }
            };

            var intentCounts = intentKeywords.ToDictionary(
                kvp => kvp.Key,
                kvp => recentMessages.Count(m => kvp.Value.Any(keyword => m.Contains(keyword)))
            );

            return intentCounts
                .OrderByDescending(kvp => kvp.Value)
                .Take(4)
                .Select(kvp => kvp.Key)
                .ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top intents");
            return new[] { "Car Maintenance", "Insurance", "Troubleshooting", "Recommendations" };
        }
    }

    private static string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;
        
        if (timeSpan.TotalMinutes < 1) return "Just now";
        if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} min ago";
        if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours}h ago";
        if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays}d ago";
        
        return dateTime.ToString("MMM dd");
    }

    private static string TruncateMessage(string? message)
    {
        if (string.IsNullOrEmpty(message)) return "No message content";
        
        const int maxLength = 50;
        if (message.Length <= maxLength) return message;
        
        return message[..maxLength] + "...";
    }
}