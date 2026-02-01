using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Services.Dashboard;

public class ReportsService : IReportsService
{
    private readonly ICurrentUserService _currentUserService;

    public ReportsService(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<List<SystemReportVM>> GetReportsAsync(int page = 1, int pageSize = 20)
    {
        // Mock data - in real implementation, query from database
        var reports = new List<SystemReportVM>();
        var reportTypes = new[] { "User Analytics", "Content Analytics", "System Performance", "Security Audit", "Financial Report" };
        var statuses = new[] { "Generated", "Generating", "Failed", "Scheduled" };
        var formats = new[] { "PDF", "Excel", "CSV", "JSON" };
        var random = new Random();

        for (int i = 0; i < pageSize; i++)
        {
            var reportType = reportTypes[random.Next(reportTypes.Length)];
            var status = statuses[random.Next(statuses.Length)];
            var format = formats[random.Next(formats.Length)];
            var createdDate = DateTime.UtcNow.AddDays(-random.Next(1, 30));

            reports.Add(new SystemReportVM
            {
                Id = Guid.NewGuid(),
                Name = $"{reportType.Replace(" ", "")}Report_{DateTime.Now:yyyyMMdd}_{i + 1}",
                Title = $"{reportType} Report #{i + 1}",
                Description = $"Comprehensive {reportType.ToLower()} report for the selected period",
                Type = reportType,
                ReportType = reportType,
                Status = status,
                CreatedAt = createdDate,
                GeneratedDate = status == "Generated" ? createdDate.AddMinutes(random.Next(5, 60)) : createdDate,
                StartDate = createdDate.AddDays(-7),
                EndDate = createdDate,
                GeneratedByName = "System Admin",
                Format = format,
                IsPublic = random.Next(0, 2) == 1,
                DownloadCount = status == "Generated" ? random.Next(0, 50) : 0,
                FileSize = status == "Generated" ? random.Next(1024, 10485760) : null,
                FileSizeFormatted = status == "Generated" ? FormatFileSize(random.Next(1024, 10485760)) : string.Empty,
                FileUrl = status == "Generated" ? $"/api/reports/download/{Guid.NewGuid()}" : null
            });
        }

        return await Task.FromResult(reports.OrderByDescending(r => r.CreatedAt).ToList());
    }

    public async Task<SystemReportVM?> GetReportByIdAsync(Guid reportId)
    {
        var reports = await GetReportsAsync(1, 100);
        return reports.FirstOrDefault(r => r.Id == reportId);
    }

    public async Task<Guid> GenerateReportAsync(ReportGenerationVM request)
    {
        // In real implementation, queue report generation job
        await Task.CompletedTask;
        return Guid.NewGuid(); // Return the ID of the generated report
    }

    public async Task<bool> DeleteReportAsync(Guid reportId)
    {
        // In real implementation, delete report from database and file system
        await Task.CompletedTask;
        return true;
    }

    public async Task<byte[]> DownloadReportAsync(Guid reportId)
    {
        // In real implementation, return actual report file bytes
        await Task.CompletedTask;
        return System.Text.Encoding.UTF8.GetBytes($"Mock report content for report {reportId}");
    }

    public async Task<List<SystemReportVM>> GetReportsByTypeAsync(string reportType, int page = 1, int pageSize = 20)
    {
        var allReports = await GetReportsAsync(1, 100);
        return allReports.Where(r => r.ReportType.Equals(reportType, StringComparison.OrdinalIgnoreCase))
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
    }

    public async Task<bool> ScheduleReportAsync(ReportScheduleVM request)
    {
        // In real implementation, schedule report generation
        await Task.CompletedTask;
        return true;
    }

    public async Task<List<SystemReportVM>> GetScheduledReportsAsync()
    {
        var allReports = await GetReportsAsync(1, 100);
        return allReports.Where(r => r.Status == "Scheduled").ToList();
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}


