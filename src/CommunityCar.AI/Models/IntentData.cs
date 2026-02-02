using Microsoft.ML.Data;

namespace CommunityCar.AI.Models;

/// <summary>
/// Intent classification data
/// </summary>
public class IntentData
{
    [LoadColumn(0)]
    public string Text { get; set; } = string.Empty;

    [LoadColumn(1), ColumnName("Label")]
    public string Intent { get; set; } = string.Empty;
}