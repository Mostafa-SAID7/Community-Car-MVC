using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

/// <summary>
/// ViewModel for updating model training settings
/// </summary>
public class UpdateModelSettingsRequestVM
{
    /// <summary>
    /// Name of the model to update
    /// </summary>
    [Required(ErrorMessage = "Model name is required")]
    [StringLength(100, ErrorMessage = "Model name cannot exceed 100 characters")]
    public string ModelName { get; set; } = string.Empty;

    /// <summary>
    /// Learning rate for training
    /// </summary>
    [Range(0.0001, 1.0, ErrorMessage = "Learning rate must be between 0.0001 and 1.0")]
    public double LearningRate { get; set; } = 0.001;

    /// <summary>
    /// Batch size for training
    /// </summary>
    [Range(1, 1000, ErrorMessage = "Batch size must be between 1 and 1000")]
    public int BatchSize { get; set; } = 32;

    /// <summary>
    /// Maximum number of training epochs
    /// </summary>
    [Range(1, 1000, ErrorMessage = "Max epochs must be between 1 and 1000")]
    public int MaxEpochs { get; set; } = 100;

    /// <summary>
    /// Whether to enable early stopping
    /// </summary>
    public bool EarlyStopping { get; set; } = true;

    /// <summary>
    /// Patience for early stopping (epochs to wait)
    /// </summary>
    [Range(1, 50, ErrorMessage = "Early stopping patience must be between 1 and 50")]
    public int EarlyStoppingPatience { get; set; } = 10;
}