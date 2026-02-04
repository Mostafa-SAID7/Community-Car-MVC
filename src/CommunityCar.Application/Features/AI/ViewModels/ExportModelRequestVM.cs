using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

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