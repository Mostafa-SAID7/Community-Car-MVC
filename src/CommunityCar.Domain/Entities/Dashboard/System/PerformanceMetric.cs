using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.System;

public class PerformanceMetric : BaseEntity
{
    public DateTime Timestamp { get; set; }
    public string MetricName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Tags { get; set; }

    public PerformanceMetric()
    {
        Timestamp = DateTime.UtcNow;
    }

    public static PerformanceMetric Create(string metricName, decimal value, string unit, string? category = null)
    {
        return new PerformanceMetric
        {
            MetricName = metricName,
            Value = value,
            Unit = unit,
            Category = category
        };
    }
}