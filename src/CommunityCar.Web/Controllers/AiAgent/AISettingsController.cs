using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Services.AI;
using CommunityCar.Application.Features.AI.ViewModels;

namespace CommunityCar.Web.Controllers.AiAgent;

[Route("AiAgent/Settings")]
public class AISettingsController : Controller
{
    private readonly IAIManagementService _aiManagementService;
    private readonly ILogger<AISettingsController> _logger;

    public AISettingsController(
        IAIManagementService aiManagementService,
        ILogger<AISettingsController> logger)
    {
        _aiManagementService = aiManagementService;
        _logger = logger;
    }

    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var settings = await _aiManagementService.GetAISettingsAsync();
            var providers = await _aiManagementService.GetAIProvidersAsync();

            var settingsObj = (dynamic)settings;
            var providersArray = providers.ToArray();

            var model = new
            {
                Providers = providersArray.Select(p => 
                {
                    var provider = (dynamic)p;
                    return new 
                    { 
                        Name = provider.Name, 
                        Status = provider.IsActive ? "Active" : "Inactive", 
                        ResponseTime = $"{provider.AverageResponseTime:F1}s", 
                        Accuracy = provider.Accuracy 
                    };
                }).ToArray(),
                Configuration = new
                {
                    DefaultProvider = settingsObj.DefaultProvider,
                    MaxResponseLength = settingsObj.MaxResponseLength,
                    ResponseTimeout = settingsObj.ResponseTimeout,
                    ConfidenceThreshold = settingsObj.ConfidenceThreshold,
                    ModerationEnabled = settingsObj.ModerationEnabled,
                    AutoTranslation = settingsObj.AutoTranslationEnabled,
                    SentimentAnalysis = settingsObj.SentimentAnalysisEnabled
                },
                Languages = ((object[])settingsObj.SupportedLanguages).ToArray(),
                Intents = ((object[])settingsObj.SupportedIntents).ToArray()
            };

            return View("~/Views/AiAgent/Settings/Index.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading AI Settings");
            
            // Return fallback data
            var fallbackModel = new
            {
                Providers = new[]
                {
                    new { Name = "Gemini", Status = "Active", ResponseTime = "0.8s", Accuracy = 94.2 },
                    new { Name = "HuggingFace", Status = "Backup", ResponseTime = "1.2s", Accuracy = 91.7 }
                },
                Configuration = new
                {
                    DefaultProvider = "Gemini",
                    MaxResponseLength = 500,
                    ResponseTimeout = 30,
                    ConfidenceThreshold = 0.7,
                    ModerationEnabled = true,
                    AutoTranslation = true,
                    SentimentAnalysis = true
                },
                Languages = new[] { "English", "Arabic", "Spanish", "French" },
                Intents = new[] { "Question", "Problem", "Greeting", "Complaint", "Appreciation" },
                Error = "Unable to load settings"
            };
            
            return View("~/Views/AiAgent/Settings/Index.cshtml", fallbackModel);
        }
    }

    [HttpPost]
    [Route("Save")]
    public async Task<IActionResult> SaveSettings([FromBody] AISettingsRequestVM request)
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

            _logger.LogInformation("Saving AI settings");

            // Validate settings ranges
            if (request.MaxResponseLength < 100 || request.MaxResponseLength > 2000)
            {
                return Json(new { success = false, message = "Max response length must be between 100 and 2000 characters" });
            }

            if (request.ResponseTimeout < 5 || request.ResponseTimeout > 120)
            {
                return Json(new { success = false, message = "Response timeout must be between 5 and 120 seconds" });
            }

            // Update settings through AI management service
            var updatedSettings = await _aiManagementService.UpdateAISettingsAsync(new
            {
                DefaultProvider = request.DefaultProvider,
                MaxResponseLength = request.MaxResponseLength,
                ResponseTimeout = request.ResponseTimeout,
                ModerationEnabled = request.ModerationEnabled,
                SentimentAnalysisEnabled = request.SentimentAnalysisEnabled,
                UpdatedBy = User.Identity?.Name ?? "System",
                UpdatedAt = DateTime.UtcNow
            });

            _logger.LogInformation("AI settings saved successfully");

            var updatedSettingsObj = (dynamic)updatedSettings;
            return Json(new
            {
                success = true,
                message = "AI settings saved successfully",
                data = new
                {
                    defaultProvider = updatedSettingsObj.DefaultProvider,
                    maxResponseLength = updatedSettingsObj.MaxResponseLength,
                    responseTimeout = updatedSettingsObj.ResponseTimeout,
                    confidenceThreshold = updatedSettingsObj.ConfidenceThreshold,
                    moderationEnabled = updatedSettingsObj.ModerationEnabled,
                    autoTranslationEnabled = updatedSettingsObj.AutoTranslationEnabled,
                    sentimentAnalysisEnabled = updatedSettingsObj.SentimentAnalysisEnabled,
                    lastUpdated = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving AI settings");
            return Json(new
            {
                success = false,
                message = "Failed to save AI settings",
                error = ex.Message
            });
        }
    }

    [HttpPost]
    [Route("Reset")]
    public async Task<IActionResult> ResetSettings()
    {
        try
        {
            _logger.LogInformation("Resetting AI settings to defaults");

            var defaultSettings = await _aiManagementService.ResetAISettingsToDefaultAsync();

            _logger.LogInformation("AI settings reset to defaults successfully");

            var defaultSettingsObj = (dynamic)defaultSettings;
            return Json(new
            {
                success = true,
                message = "AI settings reset to defaults successfully",
                settings = new
                {
                    defaultProvider = defaultSettingsObj.DefaultProvider,
                    maxResponseLength = defaultSettingsObj.MaxResponseLength,
                    responseTimeout = defaultSettingsObj.ResponseTimeout,
                    confidenceThreshold = defaultSettingsObj.ConfidenceThreshold,
                    moderationEnabled = defaultSettingsObj.ModerationEnabled,
                    autoTranslationEnabled = defaultSettingsObj.AutoTranslationEnabled,
                    sentimentAnalysisEnabled = defaultSettingsObj.SentimentAnalysisEnabled
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting AI settings");
            return Json(new
            {
                success = false,
                message = "Failed to reset AI settings",
                error = ex.Message
            });
        }
    }

    [HttpPost]
    [Route("Toggle")]
    public async Task<IActionResult> ToggleSetting([FromBody] ToggleSettingRequestVM request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid request data" });
            }

            _logger.LogInformation("Toggling AI setting: {SettingName} to {Value}", request.SettingName, request.Enabled);

            // Validate setting name
            var validSettings = new[] { "SentimentAnalysis", "ModerationEnabled", "AutoTranslation" };
            if (!validSettings.Contains(request.SettingName))
            {
                return Json(new { success = false, message = "Invalid setting name" });
            }

            var updatedSetting = await _aiManagementService.ToggleAISettingAsync(request.SettingName, request.Enabled);

            _logger.LogInformation("AI setting {SettingName} toggled to {Value} successfully", request.SettingName, request.Enabled);

            return Json(new
            {
                success = true,
                message = $"{request.SettingName} {(request.Enabled ? "enabled" : "disabled")} successfully",
                data = new
                {
                    settingName = request.SettingName,
                    enabled = request.Enabled,
                    updatedAt = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling AI setting {SettingName}", request.SettingName);
            return Json(new
            {
                success = false,
                message = "Failed to toggle setting",
                error = ex.Message
            });
        }
    }

    [HttpGet]
    [Route("Providers")]
    public async Task<IActionResult> GetProviders()
    {
        try
        {
            var providers = await _aiManagementService.GetAIProvidersAsync();
            
            var providerData = providers.Select(p => 
            {
                var provider = (dynamic)p;
                return new
                {
                    Name = provider.Name,
                    Status = provider.IsActive ? "Active" : "Inactive",
                    ResponseTime = $"{provider.AverageResponseTime:F1}s",
                    Accuracy = Math.Round(provider.Accuracy, 1),
                    LastHealthCheck = provider.LastHealthCheck,
                    IsHealthy = provider.IsHealthy,
                    SupportedModels = ((object[])provider.SupportedModels).Length,
                    Configuration = new
                    {
                        ApiEndpoint = provider.ApiEndpoint,
                        MaxConcurrentRequests = provider.MaxConcurrentRequests,
                        TimeoutSeconds = provider.TimeoutSeconds
                    }
                };
            }).ToArray();

            return Json(new
            {
                success = true,
                data = providerData
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting AI providers");
            return Json(new
            {
                success = false,
                message = "Failed to load AI providers",
                error = ex.Message
            });
        }
    }

    [HttpPost]
    [Route("Providers/{providerName}/Toggle")]
    public async Task<IActionResult> ToggleProvider(string providerName, [FromBody] bool enabled)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(providerName))
            {
                return Json(new { success = false, message = "Provider name is required" });
            }

            _logger.LogInformation("Toggling AI provider: {ProviderName} to {Enabled}", providerName, enabled);

            var result = await _aiManagementService.ToggleAIProviderAsync(providerName, enabled);
            
            var resultObj = (dynamic)result;
            
            if (!resultObj.Success)
            {
                return Json(new { success = false, message = resultObj.Message });
            }

            _logger.LogInformation("AI provider {ProviderName} toggled to {Enabled} successfully", providerName, enabled);

            return Json(new
            {
                success = true,
                message = $"Provider '{providerName}' {(enabled ? "enabled" : "disabled")} successfully",
                data = new
                {
                    providerName = providerName,
                    enabled = enabled,
                    updatedAt = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling AI provider {ProviderName}", providerName);
            return Json(new
            {
                success = false,
                message = $"Failed to toggle provider '{providerName}'",
                error = ex.Message
            });
        }
    }

    [HttpPost]
    [Route("Providers/{providerName}/Test")]
    public async Task<IActionResult> TestProvider(string providerName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(providerName))
            {
                return Json(new { success = false, message = "Provider name is required" });
            }

            _logger.LogInformation("Testing AI provider: {ProviderName}", providerName);

            var testResult = await _aiManagementService.TestAIProviderAsync(providerName);

            var testResultObj = (dynamic)testResult;

            return Json(new
            {
                success = testResultObj.IsSuccessful,
                message = testResultObj.IsSuccessful ? $"Provider '{providerName}' is working correctly" : $"Provider '{providerName}' test failed",
                data = new
                {
                    providerName = providerName,
                    responseTime = $"{testResultObj.ResponseTime:F2}s",
                    isHealthy = testResultObj.IsSuccessful,
                    testMessage = testResultObj.TestMessage,
                    errorDetails = testResultObj.ErrorMessage,
                    testedAt = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing AI provider {ProviderName}", providerName);
            return Json(new
            {
                success = false,
                message = $"Failed to test provider '{providerName}'",
                error = ex.Message
            });
        }
    }
}