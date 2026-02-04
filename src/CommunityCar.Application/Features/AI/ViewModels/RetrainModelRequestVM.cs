using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

/// <summary>
/// ViewModel for retraining AI models
/// </summary>
public class RetrainModelRequestVM
{
    /// <summary>
    /// Name of the model to retrain
    /// </summary>
    [Required(ErrorMessage = "Model name is required")]
    [StringLength(100, ErrorMessage = "Model name cannot exceed 100 characters")]
    public string ModelName { get; set; } = string.Empty;

    /// <summary>
    /// Optional description for the retraining job
    /// </summary>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Whether to use incremental training
    /// </summary>
    public bool IncrementalTraining { get; set; } = true;
}