namespace CommunityCar.AI.Models;

/// <summary>
/// Training data metrics
/// </summary>
public class TrainingMetrics
{
    public string ModelType { get; set; } = string.Empty;
    public double Accuracy { get; set; }
    public double Precision { get; set; }
    public double Recall { get; set; }
    public double F1Score { get; set; }
    public double LogLoss { get; set; }
    public DateTime TrainingDate { get; set; }
    public int DatasetSize { get; set; }
    public string ModelVersion { get; set; } = string.Empty;
    public TimeSpan TrainingTime { get; set; }
    public DateTime LastTrained { get; set; }
}