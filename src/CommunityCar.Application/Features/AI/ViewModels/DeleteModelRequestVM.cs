using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

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