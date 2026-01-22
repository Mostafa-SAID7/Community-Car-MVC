using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.Reports;

public class SystemReport : BaseEntity
{
    public string ReportType { get; set; } = string.Empty; // Daily, Weekly, Monthly, Custom
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ReportData { get; set; } = string.Empty; // JSON data
    public string Status { get; set; } = string.Empty; // Pending, Completed, Failed
    public Guid GeneratedBy { get; set; }
    public string? FilePath { get; set; }
    public long? FileSize { get; set; }
    public string? Format { get; set; } // PDF, Excel, CSV
    public bool IsPublic { get; set; }
    public int DownloadCount { get; set; }

    public SystemReport()
    {
        GeneratedDate = DateTime.UtcNow;
        Status = "Pending";
        IsPublic = false;
        DownloadCount = 0;
    }

    public void MarkAsCompleted(string filePath, long fileSize, string format)
    {
        Status = "Completed";
        FilePath = filePath;
        FileSize = fileSize;
        Format = format;
        Audit(UpdatedBy);
    }

    public void MarkAsFailed()
    {
        Status = "Failed";
        Audit(UpdatedBy);
    }

    public void IncrementDownload()
    {
        DownloadCount++;
        Audit(UpdatedBy);
    }
}