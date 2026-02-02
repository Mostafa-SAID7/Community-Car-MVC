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

/// <summary>
/// ViewModel for exporting AI models
/// </summary>
public class ExportModelRequestVM
{
    /// <summary>
    /// Name of the model to export
    /// </summary>
    [Required(ErrorMessage = "Model name is required")]
    [StringLength(100, ErrorMessage = "Model name cannot exceed 100 characters")]
    public string ModelName { get; set; } = string.Empty;

    /// <summary>
    /// Export format (e.g., "ONNX", "TensorFlow", "PyTorch")
    /// </summary>
    [Required(ErrorMessage = "Export format is required")]
    [StringLength(20, ErrorMessage = "Export format cannot exceed 20 characters")]
    public string ExportFormat { get; set; } = "ONNX";

    /// <summary>
    /// Whether to include training metadata
    /// </summary>
    public bool IncludeMetadata { get; set; } = true;

    /// <summary>
    /// Whether to compress the exported model
    /// </summary>
    public bool CompressModel { get; set; } = true;
}

/// <summary>
/// ViewModel for deleting AI models
/// </summary>
public class DeleteModelRequestVM
{
    /// <summary>
    /// Name of the model to delete
    /// </summary>
    [Required(ErrorMessage = "Model name is required")]
    [StringLength(100, ErrorMessage = "Model name cannot exceed 100 characters")]
    public string ModelName { get; set; } = string.Empty;

    /// <summary>
    /// Confirmation that the user wants to delete the model
    /// </summary>
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must confirm the deletion")]
    public bool ConfirmDeletion { get; set; }

    /// <summary>
    /// Optional reason for deletion
    /// </summary>
    [StringLength(500, ErrorMessage = "Deletion reason cannot exceed 500 characters")]
    public string? DeletionReason { get; set; }

    /// <summary>
    /// Whether to also delete associated training data
    /// </summary>
    public bool DeleteTrainingData { get; set; }
}