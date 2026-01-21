using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.Analytics;

public class Metric : BaseEntity
{
    public string Name { get; private set; }
    public double Value { get; private set; }
    public string Unit { get; private set; }
    public string Category { get; private set; }
    public DateTime Timestamp { get; private set; }

    public Metric(string name, double value, string unit, string category)
    {
        Name = name;
        Value = value;
        Unit = unit;
        Category = category;
        Timestamp = DateTime.UtcNow;
    }
}
