using Microsoft.AspNetCore.Mvc;
using CommunityCar.AI.Services;
using CommunityCar.AI.Models;

namespace CommunityCar.Web.Controllers.Dashboard;

[Route("Dashboard/AIManagement")]
public class AIManagementController : Controller
{
    private readonly IIntelligentChatService _chatService;
    private readonly ISentimentAnalysisService _sentimentService;
    private readonly IPredictionService _predictionService;
    private readonly ILogger<AIManagementController> _logger;

    public AIManagementController(
        IIntelligentChatService chatService,
        ISentimentAnalysisService sentimentService,
        IPredictionService predictionService,
        ILogger<AIManagementController> logger)
    {
        _chatService = chatService;
        _sentimentService = sentimentService;
        _predictionService = predictionService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var model = new
            {
                TotalConversations = 156,
                ActiveUsers = 89,
                ResponseTime = "1.2s",
                SatisfactionRate = 94.5,
                TodayMessages = 234,
                WeeklyGrowth = 12.3,
                TopIntents = new[] { "Car Maintenance", "Insurance", "Troubleshooting", "Recommendations" },
                RecentActivity = new[]
                {
                    new { Time = "2 min ago", User = "User123", Message = "Asked about oil change intervals" },
                    new { Time = "5 min ago", User = "User456", Message = "Requested car insurance advice" },
                    new { Time = "8 min ago", User = "User789", Message = "Troubleshooting engine noise" }
                }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading AI Management dashboard");
            return View(new { Error = "Unable to load dashboard data" });
        }
    }

    [HttpGet("Analytics")]
    public async Task<IActionResult> Analytics()
    {
        try
        {
            var model = new
            {
                ConversationMetrics = new
                {
                    Total = 1250,
                    ThisWeek = 89,
                    AvgLength = 8.5,
                    CompletionRate = 92.3
                },
                SentimentAnalysis = new
                {
                    Positive = 68.5,
                    Neutral = 25.2,
                    Negative = 6.3
                },
                TopTopics = new[]
                {
                    new { Topic = "Car Maintenance", Count = 345, Percentage = 27.6 },
                    new { Topic = "Insurance", Count = 289, Percentage = 23.1 },
                    new { Topic = "Troubleshooting", Count = 234, Percentage = 18.7 },
                    new { Topic = "Recommendations", Count = 198, Percentage = 15.8 }
                },
                UserEngagement = new
                {
                    ActiveUsers = 456,
                    ReturnRate = 78.9,
                    AvgSessionTime = "12m 34s"
                }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading AI Analytics");
            return View(new { Error = "Unable to load analytics data" });
        }
    }

    [HttpGet("Settings")]
    public async Task<IActionResult> Settings()
    {
        try
        {
            var model = new
            {
                Providers = new[]
                {
                    new { Name = "Gemini", Status = "Active", ResponseTime = "0.8s", Accuracy = 94.2 },
                    new { Name = "HuggingFace", Status = "Backup", ResponseTime = "1.2s", Accuracy = 91.7 }
                },
                Configuration = new
                {
                    MaxResponseLength = 500,
                    ConfidenceThreshold = 0.7,
                    ModerationEnabled = true,
                    AutoTranslation = true,
                    SentimentAnalysis = true
                },
                Languages = new[] { "English", "Arabic", "Spanish", "French" },
                Intents = new[] { "Question", "Problem", "Greeting", "Complaint", "Appreciation" }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading AI Settings");
            return View(new { Error = "Unable to load settings" });
        }
    }

    [HttpGet("Conversations")]
    public async Task<IActionResult> Conversations(int page = 1, int pageSize = 20)
    {
        try
        {
            // Sample conversation data - in real implementation, this would come from database
            var conversations = new[]
            {
                new {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Title = "Car maintenance schedule inquiry",
                    MessageCount = 12,
                    LastActivity = DateTime.UtcNow.AddMinutes(-15),
                    Status = "Completed",
                    Sentiment = "Positive"
                },
                new {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Title = "Insurance coverage questions",
                    MessageCount = 8,
                    LastActivity = DateTime.UtcNow.AddHours(-2),
                    Status = "Active",
                    Sentiment = "Neutral"
                },
                new {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Title = "Engine troubleshooting help",
                    MessageCount = 15,
                    LastActivity = DateTime.UtcNow.AddHours(-4),
                    Status = "Resolved",
                    Sentiment = "Negative"
                }
            };

            var model = new
            {
                Conversations = conversations,
                TotalCount = conversations.Length,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(conversations.Length / (double)pageSize)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading conversations");
            return View(new { Error = "Unable to load conversations" });
        }
    }

    [HttpGet("Conversations/{id}")]
    public async Task<IActionResult> ConversationDetails(Guid id)
    {
        try
        {
            // Sample conversation details - in real implementation, this would come from database
            var conversation = new
            {
                Id = id,
                UserId = Guid.NewGuid(),
                Title = "Car maintenance schedule inquiry",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-15),
                Status = "Completed",
                Messages = new[]
                {
                    new {
                        Id = Guid.NewGuid(),
                        Content = "Hi, I need help with my car maintenance schedule",
                        IsFromUser = true,
                        Timestamp = DateTime.UtcNow.AddDays(-2),
                        Sentiment = "Neutral"
                    },
                    new {
                        Id = Guid.NewGuid(),
                        Content = "I'd be happy to help you with your car maintenance schedule! What type of vehicle do you have and what's your current mileage?",
                        IsFromUser = false,
                        Timestamp = DateTime.UtcNow.AddDays(-2).AddMinutes(1),
                        Sentiment = "Positive"
                    },
                    new {
                        Id = Guid.NewGuid(),
                        Content = "I have a 2020 Honda Civic with 35,000 miles",
                        IsFromUser = true,
                        Timestamp = DateTime.UtcNow.AddDays(-2).AddMinutes(2),
                        Sentiment = "Neutral"
                    },
                    new {
                        Id = Guid.NewGuid(),
                        Content = "Great! For your 2020 Honda Civic at 35,000 miles, here's what you should consider: Oil changes every 5,000-7,500 miles, tire rotation every 5,000-7,500 miles, and brake inspection every 12,000 miles. You're also approaching the time for your 36,000-mile service which typically includes transmission fluid change and spark plug replacement.",
                        IsFromUser = false,
                        Timestamp = DateTime.UtcNow.AddDays(-2).AddMinutes(3),
                        Sentiment = "Positive"
                    }
                }
            };

            return View(conversation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading conversation details for {ConversationId}", id);
            return View(new { Error = "Unable to load conversation details" });
        }
    }

    [HttpGet("Training")]
    public async Task<IActionResult> Training()
    {
        try
        {
            var model = new
            {
                Models = new[]
                {
                    new {
                        Name = "Intent Classification",
                        Version = "v2.1",
                        Accuracy = 94.2,
                        LastTrained = DateTime.UtcNow.AddDays(-7),
                        Status = "Active",
                        DatasetSize = 15000
                    },
                    new {
                        Name = "Sentiment Analysis",
                        Version = "v1.8",
                        Accuracy = 91.7,
                        LastTrained = DateTime.UtcNow.AddDays(-5),
                        Status = "Active",
                        DatasetSize = 12000
                    },
                    new {
                        Name = "Response Generation",
                        Version = "v3.0",
                        Accuracy = 89.3,
                        LastTrained = DateTime.UtcNow.AddDays(-3),
                        Status = "Training",
                        DatasetSize = 8500
                    }
                },
                TrainingQueue = new[]
                {
                    new { Model = "Intent Classification", Status = "Queued", EstimatedTime = "2h 15m" },
                    new { Model = "Sentiment Analysis", Status = "In Progress", EstimatedTime = "45m" }
                },
                RecentTraining = new[]
                {
                    new { Model = "Response Generation", Date = DateTime.UtcNow.AddDays(-1), Result = "Success", Improvement = "+2.1%" },
                    new { Model = "Intent Classification", Date = DateTime.UtcNow.AddDays(-3), Result = "Success", Improvement = "+1.8%" }
                }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading training data");
            return View(new { Error = "Unable to load training data" });
        }
    }

    [HttpGet("Test")]
    public async Task<IActionResult> Test()
    {
        return View();
    }

    [HttpPost("Test")]
    public async Task<IActionResult> TestMessage([FromBody] TestMessageRequest request)
    {
        try
        {
            var response = await _chatService.GetResponseAsync(
                request.Message, 
                Guid.NewGuid(), 
                request.Context
            );

            return Json(new
            {
                success = true,
                data = new
                {
                    response = response.Message,
                    confidence = response.Confidence,
                    sentiment = response.SentimentAnalysis?.Label ?? "Unknown",
                    suggestions = response.Suggestions,
                    processingTime = "0.8s"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing AI message");
            return Json(new
            {
                success = false,
                message = "Error processing test message"
            });
        }
    }
}

public class TestMessageRequest
{
    public string Message { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
}