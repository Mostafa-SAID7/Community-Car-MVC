using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.Analytics;

public class GeneratedReport : BaseEntity
{
    public string Title { get; private set; }
    public string ReportType { get; private set; }
    public string DataJson { get; private set; } // Storing report data as JSON for flexibility
    public Guid GeneratedBy { get; private set; }

    public GeneratedReport(string title, string reportType, string dataJson, Guid generatedBy)
    {
        Title = title;
        ReportType = reportType;
        DataJson = dataJson;
        GeneratedBy = generatedBy;
    }
}