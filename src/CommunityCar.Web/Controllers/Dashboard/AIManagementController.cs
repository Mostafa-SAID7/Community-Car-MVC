using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.AI.Services;
using CommunityCar.AI.Configuration;
using Microsoft.Extensions.Options;
using CommunityCar.Application.Common.Interfaces.Services.Identity;

namespace CommunityCar.Web.Controllers.Dashboard;

[Authorize(Roles = "Admin,SuperAdmin")]
[Route("Dashboard/AI")]
public class AIManagementController : Controller
{
    private readonly IIntelligentChatService _chatService;
    private readonly ISentimentAnalysisService _sentimentService;
    private readonly IPredictionService _predictionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly AISettings _aiSettings;
    private readonly ILogger<AIManagementController> _logger;

    public AIManagementController(
        IIntelligentChatService chatService,
        ISentimentAnalysisService sentimentService,
        IPredictionService predictionService,
        ICurrentUserService currentUserService,
        IOptions<AISettings> aiSettings,
        ILogger<AIManagementController> logger)
    {
        _chatService = chatService;
        _sentimentService = sentimentService;
        _predictionService = predictionService;
        _currentUserService = currentUserService;
        _aiSettings = aiSettings.Value;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "AI Management";
        
        var model = new AIManagementViewModel
        {
            CurrentProvider = _aiSettings.DefaultProvider,
            GeminiConfigured = !string.IsNullOrEmpty(_aiSettings.Gemini.ApiKey),
            HuggingFaceConfigured = !string.IsNullOrEmpty(_aiSettings.HuggingFace.ApiKey),
            TotalConversations = await GetTotalConversationsAsync(),
            TotalMessages = await GetTotalMessagesAsync(),
            AverageResponseTime = await GetAverageResponseTimeAsync(),
            TopTopics = await GetTopTopicsAsync(),
            SentimentDistribution = await GetSentimentDistributionAsync()
        };

        return View(model);
    }

    [HttpGet("Analytics")]
    public async Task<IActionResult> Analytics()
    {
        ViewData["Title"] = "AI Analytics";
        
        var model = new AIAnalyticsViewModel
        {
            ConversationStats = await GetConversationStatsAsync(),
            MessageStats = await GetMessageStatsAsync(),
            SentimentTrends = await GetSentimentTrendsAsync(),
            TopicAnalysis = await GetTopicAnalysisAsync(),
            UserEngagement = await GetUserEngagementAsync(),
            PerformanceMetrics = await GetPerformanceMetricsAsync()
        };

        return View(model);
    }

    [HttpGet("Settings")]
    public IActionResult Settings()
    {
        ViewData["Title"] = "AI Settings";
        
        var model = new AISettingsViewModel
        {
            DefaultProvider = _aiSettings.DefaultProvider,
            GeminiApiKey = _aiSettings.Gemini.ApiKey,
            HuggingFaceApiKey = _aiSettings.HuggingFace.ApiKey,
            EnableSentimentAnalysis = true,
            EnablePredictions = true,
            EnableModeration = true,
            MaxResponseLength = 500,
            ResponseTimeout = 30,
            EnableLogging = true
        };

        return View(model);
    }

    [HttpPost("Settings")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(AISettingsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // In a real implementation, you would update the configuration
            // For now, we'll just show a success message
            TempData["Success"] = "AI settings updated successfully";
            return RedirectToAction(nameof(Settings));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating AI settings");
            ModelState.AddModelError("", "Failed to update AI settings");
            return View(model);
        }
    }

    [HttpGet("Conversations")]
    public async Task<IActionResult> Conversations(int page = 1, int pageSize = 20)
    {
        ViewData["Title"] = "AI Conversations";
        
        var model = new AIConversationsViewModel
        {
            Conversations = await GetConversationsAsync(page, pageSize),
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = await GetTotalConversationsAsync()
        };

        return View(model);
    }

    [HttpGet("Conversation/{id}")]
    public async Task<IActionResult> ConversationDetails(Guid id)
    {
        ViewData["Title"] = "Conversation Details";
        
        var conversation = await GetConversationDetailsAsync(id);
        if (conversation == null)
        {
            return NotFound();
        }

        return View(conversation);
    }

    [HttpGet("Training")]
    public async Task<IActionResult> Training()
    {
        ViewData["Title"] = "AI Training";
        
        var model = new AITrainingViewModel
        {
            TrainingDataCount = await GetTrainingDataCountAsync(),
            LastTrainingDate = await GetLastTrainingDateAsync(),
            ModelAccuracy = await GetModelAccuracyAsync(),
            PendingFeedback = await GetPendingFeedbackCountAsync()
        };

        return View(model);
    }

    [HttpPost("Training/Start")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartTraining()
    {
        try
        {
            // In a real implementation, you would start the training process
            await Task.Delay(100); // Simulate training start
            
            TempData["Success"] = "AI training started successfully";
            return RedirectToAction(nameof(Training));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting AI training");
            TempData["Error"] = "Failed to start AI training";
            return RedirectToAction(nameof(Training));
        }
    }

    [HttpGet("Test")]
    public IActionResult Test()
    {
        ViewData["Title"] = "AI Test Console";
        return View();
    }

    [HttpPost("Test")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TestMessage([FromBody] TestMessageRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var response = await _chatService.GetResponseAsync(request.Message, userId, request.Context);
            
            return Json(new
            {
                success = true,
                data = new
                {
                    response = response.Message,
                    confidence = response.Confidence,
                    suggestions = response.Suggestions,
                    topics = response.ExtractedTopics,
                    entities = response.ExtractedEntities,
                    timestamp = response.Timestamp
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing AI message");
            return Json(new { success = false, message = "Failed to process test message" });
        }
    }

    // Private helper methods
    private async Task<int> GetTotalConversationsAsync()
    {
        // Mock data - in real implementation, query database
        return await Task.FromResult(1250);
    }

    private async Task<int> GetTotalMessagesAsync()
    {
        return await Task.FromResult(8750);
    }

    private async Task<TimeSpan> GetAverageResponseTimeAsync()
    {
        return await Task.FromResult(TimeSpan.FromMilliseconds(850));
    }

    private async Task<List<string>> GetTopTopicsAsync()
    {
        return await Task.FromResult(new List<string>
        {
            "Car Maintenance", "Insurance", "Electric Vehicles", "Repairs", "Safety"
        });
    }

    private async Task<Dictionary<string, int>> GetSentimentDistributionAsync()
    {
        return await Task.FromResult(new Dictionary<string, int>
        {
            { "Positive", 65 },
            { "Neutral", 25 },
            { "Negative", 10 }
        });
    }

    private async Task<object> GetConversationStatsAsync()
    {
        return await Task.FromResult(new
        {
            Daily = new[] { 45, 52, 38, 67, 73, 58, 62 },
            Weekly = new[] { 320, 385, 290, 410, 375, 445, 390 },
            Monthly = new[] { 1250, 1380, 1150, 1420, 1320, 1580, 1450 }
        });
    }

    private async Task<object> GetMessageStatsAsync()
    {
        return await Task.FromResult(new
        {
            Hourly = Enumerable.Range(0, 24).Select(h => new Random().Next(10, 100)).ToArray(),
            ResponseTimes = new[] { 0.5, 0.8, 1.2, 0.9, 1.1, 0.7, 0.6, 1.0, 0.8, 1.3 }
        });
    }

    private async Task<object> GetSentimentTrendsAsync()
    {
        return await Task.FromResult(new
        {
            Positive = new[] { 60, 65, 62, 68, 70, 67, 65 },
            Neutral = new[] { 30, 25, 28, 22, 20, 23, 25 },
            Negative = new[] { 10, 10, 10, 10, 10, 10, 10 }
        });
    }

    private async Task<object> GetTopicAnalysisAsync()
    {
        return await Task.FromResult(new
        {
            Topics = new[] { "Maintenance", "Insurance", "Electric", "Repairs", "Safety", "Performance" },
            Counts = new[] { 450, 320, 280, 380, 220, 180 }
        });
    }

    private async Task<object> GetUserEngagementAsync()
    {
        return await Task.FromResult(new
        {
            ActiveUsers = 1250,
            NewUsers = 85,
            ReturnUsers = 920,
            AverageSessionLength = 12.5
        });
    }

    private async Task<object> GetPerformanceMetricsAsync()
    {
        return await Task.FromResult(new
        {
            AverageResponseTime = 0.85,
            SuccessRate = 98.5,
            ErrorRate = 1.5,
            Uptime = 99.9
        });
    }

    private async Task<List<object>> GetConversationsAsync(int page, int pageSize)
    {
        // Mock data
        return await Task.FromResult(Enumerable.Range(1, pageSize).Select(i => new
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = $"Conversation {i + (page - 1) * pageSize}",
            MessageCount = new Random().Next(5, 50),
            LastActivity = DateTime.UtcNow.AddHours(-new Random().Next(1, 72)),
            Sentiment = new[] { "Positive", "Neutral", "Negative" }[new Random().Next(3)]
        }).ToList<object>());
    }

    private async Task<object?> GetConversationDetailsAsync(Guid id)
    {
        return await Task.FromResult(new
        {
            Id = id,
            UserId = Guid.NewGuid(),
            Title = "Sample Conversation",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Messages = Enumerable.Range(1, 10).Select(i => new
            {
                Id = Guid.NewGuid(),
                IsFromUser = i % 2 == 1,
                Content = i % 2 == 1 ? $"User message {i}" : $"AI response {i}",
                Timestamp = DateTime.UtcNow.AddDays(-2).AddMinutes(i * 5),
                Sentiment = "Positive"
            }).ToList()
        });
    }

    private async Task<int> GetTrainingDataCountAsync()
    {
        return await Task.FromResult(15000);
    }

    private async Task<DateTime?> GetLastTrainingDateAsync()
    {
        return await Task.FromResult(DateTime.UtcNow.AddDays(-7));
    }

    private async Task<double> GetModelAccuracyAsync()
    {
        return await Task.FromResult(94.5);
    }

    private async Task<int> GetPendingFeedbackCountAsync()
    {
        return await Task.FromResult(45);
    }
}

// View Models
public class AIManagementViewModel
{
    public string CurrentProvider { get; set; } = string.Empty;
    public bool GeminiConfigured { get; set; }
    public bool HuggingFaceConfigured { get; set; }
    public int TotalConversations { get; set; }
    public int TotalMessages { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public List<string> TopTopics { get; set; } = new();
    public Dictionary<string, int> SentimentDistribution { get; set; } = new();
}

public class AIAnalyticsViewModel
{
    public object? ConversationStats { get; set; }
    public object? MessageStats { get; set; }
    public object? SentimentTrends { get; set; }
    public object? TopicAnalysis { get; set; }
    public object? UserEngagement { get; set; }
    public object? PerformanceMetrics { get; set; }
}

public class AISettingsViewModel
{
    public string DefaultProvider { get; set; } = string.Empty;
    public string GeminiApiKey { get; set; } = string.Empty;
    public string HuggingFaceApiKey { get; set; } = string.Empty;
    public bool EnableSentimentAnalysis { get; set; }
    public bool EnablePredictions { get; set; }
    public bool EnableModeration { get; set; }
    public int MaxResponseLength { get; set; }
    public int ResponseTimeout { get; set; }
    public bool EnableLogging { get; set; }
}

public class AIConversationsViewModel
{
    public List<object> Conversations { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public class AITrainingViewModel
{
    public int TrainingDataCount { get; set; }
    public DateTime? LastTrainingDate { get; set; }
    public double ModelAccuracy { get; set; }
    public int PendingFeedback { get; set; }
}

public class TestMessageRequest
{
    public string Message { get; set; } = string.Empty;
    public string? Context { get; set; }
}