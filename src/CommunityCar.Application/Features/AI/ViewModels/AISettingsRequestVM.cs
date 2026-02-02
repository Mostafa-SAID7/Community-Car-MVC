using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

/// <summary>
/// ViewModel for AI settings configuration
/// </summary>
public class AISettingsRequestVM
{
    /// <summary>
    /// Default AI provider to use
    /// </summary>
    [Required(ErrorMessage = "Default provider is required")]
    [StringLength(50, ErrorMessage = "Provider name cannot exceed 50 characters")]
    public string DefaultProvider { get; set; } = string.Empty;

    /// <summary>
    /// Maximum response length in characters
    /// </summary>
    [Range(100, 10000, ErrorMessage = "Max response length must be between 100 and 10000 characters")]
    public int MaxResponseLength { get; set; } = 2000;

    /// <summary>
    /// Response timeout in seconds
    /// </summary>
    [Range(5, 300, ErrorMessage = "Response timeout must be between 5 and 300 seconds")]
    public int ResponseTimeout { get; set; } = 30;

    /// <summary>
    /// Confidence threshold for AI responses
    /// </summary>
    [Range(0.1, 1.0, ErrorMessage = "Confidence threshold must be between 0.1 and 1.0")]
    public double ConfidenceThreshold { get; set; } = 0.7;

    /// <summary>
    /// Whether sentiment analysis is enabled
    /// </summary>
    public bool SentimentAnalysisEnabled { get; set; } = true;

    /// <summary>
    /// Whether content moderation is enabled
    /// </summary>
    public bool ModerationEnabled { get; set; } = true;

    /// <summary>
    /// Whether auto translation is enabled
    /// </summary>
    public bool AutoTranslationEnabled { get; set; } = true;
}

/// <summary>
/// ViewModel for toggling individual AI settings
/// </summary>
public class ToggleSettingRequestVM
{
    /// <summary>
    /// Name of the setting to toggle
    /// </summary>
    [Required(ErrorMessage = "Setting name is required")]
    [StringLength(100, ErrorMessage = "Setting name cannot exceed 100 characters")]
    public string SettingName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the setting should be enabled
    /// </summary>
    public bool Enabled { get; set; }
}