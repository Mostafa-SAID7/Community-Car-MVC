using Microsoft.AspNetCore.Mvc;
using CommunityCar.AI.Services;
using CommunityCar.AI.Models;
using CommunityCar.Application.Services.AI;
using CommunityCar.Application.Features.AI.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Controllers.AiAgent;

[Route("AiAgent/Testing")]
public class AITestingController : Controller
{
    private readonly IIntelligentChatService _chatService;
    private readonly ISentimentAnalysisService _sentimentService;
    private readonly IAIManagementService _aiManagementService;
    private readonly ILogger<AITestingController> _logger;

    public AITestingController(
        IIntelligentChatService chatService,
        ISentimentAnalysisService sentimentService,
        IAIManagementService aiManagementService,
        ILogger<AITestingController> logger)
    {
        _chatService = chatService;
        _sentimentService = sentimentService;
        _aiManagementService = aiManagementService;
        _logger = logger;
    }

    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var models = await _aiManagementService.GetAllModelsAsync();
            var providers = await _aiManagementService.GetAIProvidersAsync();

            var testingData = new
            {
                AvailableModels = models.Select(m => new
                {
                    Id = m.Id,
                    Name = m.Name,
                    Type = m.Type.ToString(),
                    Status = m.Status.ToString(),
                    Accuracy = Math.Round(m.Accuracy, 2),
                    IsActive = m.IsActive,
                    LastTrained = m.LastTrained
                }).ToArray(),
                
                AvailableProviders = providers.Select(p => 
                {
                    var provider = (dynamic)p;
                    return new
                    {
                        Name = provider.Name,
                        IsActive = provider.IsActive,
                        IsHealthy = provider.IsHealthy,
                        ResponseTime = $"{provider.AverageResponseTime:F0}ms"
                    };
                }).ToArray(),

                TestCategories = new[]
                {
                    new { Name = "Response Quality", Description = "Test AI response accuracy and relevance" },
                    new { Name = "Sentiment Analysis", Description = "Test sentiment detection capabilities" },
                    new { Name = "Intent Recognition", Description = "Test intent classification accuracy" },
                    new { Name = "Performance", Description = "Test response time and throughput" },
                    new { Name = "Moderation", Description = "Test content filtering and safety" }
                },

                RecentTests = await GetRecentTestResults()
            };

            return View("~/Views/AiAgent/Testing/Index.cshtml", testingData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading testing dashboard");
            
            var errorModel = new
            {
                AvailableModels = new object[0],
                AvailableProviders = new object[0],
                TestCategories = new object[0],
                RecentTests = new object[0],
                Error = "Unable to load testing data"
            };
            
            return View("~/Views/AiAgent/Testing/Index.cshtml", errorModel);
        }
    }

    [HttpPost]
    [Route("Message")]
    public async Task<IActionResult> TestMessage([FromBody] TestMessageRequestVM request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return Json(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = errors
                });
            }

            _logger.LogInformation("Testing AI message: {Message}", request.TrimmedMessage);

            var startTime = DateTime.UtcNow;
            
            // Test the AI chat service
            var chatRequest = new ChatRequest
            {
                Message = request.TrimmedMessage,
                Context = request.TrimmedContext,
                UserId = Guid.NewGuid().ToString(), // Test user ID
                ConversationId = Guid.NewGuid().ToString() // Test conversation ID
            };
            var chatResponse = await _chatService.ProcessMessageAsync(chatRequest);
            
            var responseTime = DateTime.UtcNow - startTime;

            // Analyze the response quality
            var qualityMetrics = await AnalyzeResponseQuality(request.TrimmedMessage, chatResponse.Message);
            
            // Test sentiment analysis if available
            var sentimentResult = await TestSentimentAnalysis(request.TrimmedMessage);

            var testResult = new
            {
                TestId = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                
                Input = new
                {
                    Message = request.TrimmedMessage,
                    Context = request.TrimmedContext,
                    WordCount = request.MessageWordCount
                },
                
                Output = new
                {
                    Response = chatResponse.Message,
                    Confidence = chatResponse.Confidence,
                    Intent = chatResponse.ExtractedTopics?.FirstOrDefault() ?? "Unknown",
                    ResponseTime = $"{responseTime.TotalMilliseconds:F0}ms",
                    WordCount = CountWords(chatResponse.Message)
                },
                
                QualityMetrics = qualityMetrics,
                SentimentAnalysis = sentimentResult,
                
                Performance = new
                {
                    ResponseTimeMs = (int)responseTime.TotalMilliseconds,
                    IsWithinThreshold = responseTime.TotalSeconds < 5,
                    TokensPerSecond = CalculateTokensPerSecond(chatResponse.Message, responseTime)
                }
            };

            _logger.LogInformation("AI message test completed successfully in {ResponseTime}ms", responseTime.TotalMilliseconds);

            return Json(new
            {
                success = true,
                message = "AI message test completed successfully",
                data = testResult
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing AI message");
            return Json(new
            {
                success = false,
                message = "Failed to test AI message",
                error = ex.Message
            });
        }
    }

    [HttpPost]
    [Route("Batch")]
    public async Task<IActionResult> RunBatchTest([FromBody] BatchTestRequestVM request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return Json(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = errors
                });
            }

            _logger.LogInformation("Running batch test with {Count} messages", request.TestMessages.Count);

            var batchResults = new List<object>();
            var startTime = DateTime.UtcNow;
            var successCount = 0;
            var failureCount = 0;

            foreach (var testMessage in request.TestMessages)
            {
                try
                {
                    var messageStartTime = DateTime.UtcNow;
                    var chatRequest = new ChatRequest
                    {
                        Message = testMessage.Message,
                        Context = testMessage.Context ?? "",
                        UserId = Guid.NewGuid().ToString(),
                        ConversationId = Guid.NewGuid().ToString()
                    };
                    var response = await _chatService.ProcessMessageAsync(chatRequest);
                    var messageResponseTime = DateTime.UtcNow - messageStartTime;

                    var result = new
                    {
                        Input = testMessage.Message,
                        Output = response.Message,
                        ResponseTime = $"{messageResponseTime.TotalMilliseconds:F0}ms",
                        Confidence = response.Confidence,
                        Intent = response.ExtractedTopics?.FirstOrDefault() ?? "Unknown",
                        Success = true,
                        Error = (string?)null
                    };

                    batchResults.Add(result);
                    successCount++;
                }
                catch (Exception ex)
                {
                    var result = new
                    {
                        Input = testMessage.Message,
                        Output = "",
                        ResponseTime = "0ms",
                        Confidence = 0.0,
                        Intent = "",
                        Success = false,
                        Error = ex.Message
                    };

                    batchResults.Add(result);
                    failureCount++;
                }
            }

            var totalTime = DateTime.UtcNow - startTime;
            var avgResponseTime = batchResults
                .Where(r => (bool)r.GetType().GetProperty("Success")?.GetValue(r)!)
                .Select(r => double.Parse(((string)r.GetType().GetProperty("ResponseTime")?.GetValue(r)!).Replace("ms", "")))
                .DefaultIfEmpty(0)
                .Average();

            var batchTestResult = new
            {
                BatchId = Guid.NewGuid(),
                StartTime = startTime,
                EndTime = DateTime.UtcNow,
                TotalDuration = $"{totalTime.TotalSeconds:F1}s",
                
                Statistics = new
                {
                    TotalTests = request.TestMessages.Count,
                    Successful = successCount,
                    Failed = failureCount,
                    SuccessRate = Math.Round((double)successCount / request.TestMessages.Count * 100, 1),
                    AverageResponseTime = $"{avgResponseTime:F0}ms"
                },
                
                Results = batchResults.ToArray()
            };

            _logger.LogInformation("Batch test completed: {Success}/{Total} successful", successCount, request.TestMessages.Count);

            return Json(new
            {
                success = true,
                message = $"Batch test completed: {successCount}/{request.TestMessages.Count} successful",
                data = batchTestResult
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running batch test");
            return Json(new
            {
                success = false,
                message = "Failed to run batch test",
                error = ex.Message
            });
        }
    }

    [HttpPost]
    [Route("Performance")]
    public async Task<IActionResult> RunPerformanceTest([FromBody] PerformanceTestRequestVM request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid request parameters" });
            }

            _logger.LogInformation("Running performance test with {Concurrent} concurrent requests for {Duration}s", 
                request.ConcurrentRequests, request.DurationSeconds);

            var testMessage = request.TestMessage ?? "Hello, this is a performance test message.";
            var results = new List<object>();
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddSeconds(request.DurationSeconds);

            var tasks = new List<Task>();
            var requestCount = 0;
            var successCount = 0;
            var errorCount = 0;
            var responseTimes = new List<double>();

            // Create concurrent tasks
            for (int i = 0; i < request.ConcurrentRequests; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    while (DateTime.UtcNow < endTime)
                    {
                        try
                        {
                            var reqStartTime = DateTime.UtcNow;
                            var chatRequest = new ChatRequest
                            {
                                Message = testMessage,
                                Context = "Performance test context",
                                UserId = Guid.NewGuid().ToString(),
                                ConversationId = Guid.NewGuid().ToString()
                            };
                            var response = await _chatService.ProcessMessageAsync(chatRequest);
                            var reqResponseTime = DateTime.UtcNow - reqStartTime;

                            lock (results)
                            {
                                requestCount++;
                                successCount++;
                                responseTimes.Add(reqResponseTime.TotalMilliseconds);
                            }

                            // Small delay to prevent overwhelming the service
                            await Task.Delay(100);
                        }
                        catch (Exception ex)
                        {
                            lock (results)
                            {
                                requestCount++;
                                errorCount++;
                            }
                            _logger.LogWarning(ex, "Performance test request failed");
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks);
            var actualDuration = DateTime.UtcNow - startTime;

            var performanceResult = new
            {
                TestId = Guid.NewGuid(),
                Configuration = new
                {
                    ConcurrentRequests = request.ConcurrentRequests,
                    PlannedDuration = $"{request.DurationSeconds}s",
                    ActualDuration = $"{actualDuration.TotalSeconds:F1}s",
                    TestMessage = testMessage
                },
                
                Results = new
                {
                    TotalRequests = requestCount,
                    SuccessfulRequests = successCount,
                    FailedRequests = errorCount,
                    SuccessRate = requestCount > 0 ? Math.Round((double)successCount / requestCount * 100, 1) : 0,
                    RequestsPerSecond = Math.Round(requestCount / actualDuration.TotalSeconds, 1),
                    
                    ResponseTimes = responseTimes.Any() ? new
                    {
                        Average = Math.Round(responseTimes.Average(), 1),
                        Min = Math.Round(responseTimes.Min(), 1),
                        Max = Math.Round(responseTimes.Max(), 1),
                        Median = Math.Round(responseTimes.OrderBy(x => x).Skip(responseTimes.Count / 2).First(), 1),
                        P95 = Math.Round(responseTimes.OrderBy(x => x).Skip((int)(responseTimes.Count * 0.95)).First(), 1)
                    } : null
                }
            };

            _logger.LogInformation("Performance test completed: {Requests} requests, {RPS} RPS, {SuccessRate}% success rate", 
                requestCount, Math.Round(requestCount / actualDuration.TotalSeconds, 1), 
                Math.Round((double)successCount / requestCount * 100, 1));

            return Json(new
            {
                success = true,
                message = "Performance test completed successfully",
                data = performanceResult
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running performance test");
            return Json(new
            {
                success = false,
                message = "Failed to run performance test",
                error = ex.Message
            });
        }
    }

    [HttpGet]
    [Route("Results/{testId}")]
    public async Task<IActionResult> GetTestResults(Guid testId)
    {
        try
        {
            _logger.LogInformation("Retrieving test results for {TestId}", testId);

            // In a real implementation, this would query a test results repository
            await Task.Delay(100);

            var testResults = new
            {
                TestId = testId,
                TestType = "Message Test",
                Status = "Completed",
                StartTime = DateTime.UtcNow.AddMinutes(-5),
                EndTime = DateTime.UtcNow.AddMinutes(-3),
                Duration = "2m 15s",
                
                Summary = new
                {
                    TotalTests = 25,
                    Passed = 23,
                    Failed = 2,
                    SuccessRate = 92.0,
                    AverageResponseTime = "1.2s"
                },
                
                Details = new[]
                {
                    new { Category = "Response Quality", Score = 94.5, Status = "Excellent" },
                    new { Category = "Performance", Score = 87.2, Status = "Good" },
                    new { Category = "Accuracy", Score = 91.8, Status = "Very Good" },
                    new { Category = "Consistency", Score = 89.3, Status = "Good" }
                }
            };

            return Json(new
            {
                success = true,
                data = testResults
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving test results for {TestId}", testId);
            return Json(new
            {
                success = false,
                message = "Failed to retrieve test results",
                error = ex.Message
            });
        }
    }

    private async Task<object[]> GetRecentTestResults()
    {
        try
        {
            // In a real implementation, this would query a test results repository
            await Task.Delay(50);

            return new[]
            {
                new { 
                    TestId = Guid.NewGuid(), 
                    Type = "Message Test", 
                    Status = "Passed", 
                    Score = 94.2, 
                    Duration = "1.2s", 
                    RunAt = DateTime.UtcNow.AddMinutes(-15) 
                },
                new { 
                    TestId = Guid.NewGuid(), 
                    Type = "Performance Test", 
                    Status = "Passed", 
                    Score = 87.5, 
                    Duration = "30s", 
                    RunAt = DateTime.UtcNow.AddHours(-1) 
                },
                new { 
                    TestId = Guid.NewGuid(), 
                    Type = "Batch Test", 
                    Status = "Failed", 
                    Score = 65.3, 
                    Duration = "2m 45s", 
                    RunAt = DateTime.UtcNow.AddHours(-2) 
                }
            };
        }
        catch
        {
            return new object[0];
        }
    }

    private async Task<object> AnalyzeResponseQuality(string input, string response)
    {
        try
        {
            await Task.Delay(50); // Simulate analysis time

            var relevanceScore = CalculateRelevance(input, response);
            var clarityScore = CalculateClarity(response);
            var completenessScore = CalculateCompleteness(input, response);

            return new
            {
                RelevanceScore = Math.Round(relevanceScore, 1),
                ClarityScore = Math.Round(clarityScore, 1),
                CompletenessScore = Math.Round(completenessScore, 1),
                OverallScore = Math.Round((relevanceScore + clarityScore + completenessScore) / 3, 1),
                
                Analysis = new
                {
                    HasRelevantKeywords = ContainsRelevantKeywords(input, response),
                    IsWellStructured = IsWellStructured(response),
                    HasAppropriateTone = HasAppropriateTone(response),
                    LengthAppropriate = IsLengthAppropriate(response)
                }
            };
        }
        catch
        {
            return new
            {
                RelevanceScore = 0.0,
                ClarityScore = 0.0,
                CompletenessScore = 0.0,
                OverallScore = 0.0,
                Analysis = new { }
            };
        }
    }

    private async Task<object> TestSentimentAnalysis(string message)
    {
        try
        {
            var sentimentResult = await _sentimentService.AnalyzeSentimentAsync(message);
            
            return new
            {
                Label = sentimentResult.Label,
                Confidence = Math.Round(sentimentResult.Score, 3),
                IsAccurate = true, // In real implementation, this would be validated against expected results
                ProcessingTime = "45ms"
            };
        }
        catch (Exception ex)
        {
            return new
            {
                Label = "Unknown",
                Confidence = 0.0,
                IsAccurate = false,
                Error = ex.Message
            };
        }
    }

    private static double CalculateRelevance(string input, string response)
    {
        var inputWords = input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var responseWords = response.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        var commonWords = inputWords.Intersect(responseWords).Count();
        var relevanceRatio = inputWords.Length > 0 ? (double)commonWords / inputWords.Length : 0;
        
        return Math.Min(relevanceRatio * 100, 100);
    }

    private static double CalculateClarity(string response)
    {
        var sentences = response.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        var avgSentenceLength = sentences.Length > 0 ? response.Length / sentences.Length : 0;
        
        // Optimal sentence length is around 15-20 words
        var clarityScore = avgSentenceLength switch
        {
            < 50 => 95.0,   // Very clear
            < 100 => 85.0,  // Clear
            < 150 => 75.0,  // Acceptable
            < 200 => 65.0,  // Somewhat unclear
            _ => 50.0       // Unclear
        };
        
        return clarityScore;
    }

    private static double CalculateCompleteness(string input, string response)
    {
        // Simple heuristic: longer responses to complex questions are generally more complete
        var inputComplexity = input.Split(' ').Length;
        var responseLength = response.Split(' ').Length;
        
        var expectedLength = inputComplexity * 2; // Expect response to be roughly 2x input length
        var completenessRatio = Math.Min((double)responseLength / expectedLength, 1.0);
        
        return completenessRatio * 100;
    }

    private static bool ContainsRelevantKeywords(string input, string response)
    {
        var inputWords = input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var responseWords = response.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        return inputWords.Any(word => word.Length > 3 && responseWords.Contains(word));
    }

    private static bool IsWellStructured(string response)
    {
        return response.Contains('.') || response.Contains('!') || response.Contains('?');
    }

    private static bool HasAppropriateTone(string response)
    {
        var professionalWords = new[] { "please", "thank", "help", "assist", "recommend", "suggest" };
        return professionalWords.Any(word => response.ToLower().Contains(word));
    }

    private static bool IsLengthAppropriate(string response)
    {
        var wordCount = response.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        return wordCount >= 5 && wordCount <= 200; // Reasonable response length
    }

    private static int CountWords(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? 0 : text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private static double CalculateTokensPerSecond(string response, TimeSpan responseTime)
    {
        var tokenCount = CountWords(response) * 1.3; // Rough estimate: 1.3 tokens per word
        return responseTime.TotalSeconds > 0 ? tokenCount / responseTime.TotalSeconds : 0;
    }
}