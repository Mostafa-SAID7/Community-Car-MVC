using System.ComponentModel.DataAnnotations;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Community.SoftDelete.ViewModels;

public class SoftDeleteStatsVM
{
    public int TotalDeletedItems { get; set; }
    public int DeletedToday { get; set; }
    public int DeletedThisWeek { get; set; }
    public int DeletedThisMonth { get; set; }
    public int RestoredItems { get; set; }
    public int PermanentlyDeletedItems { get; set; }
    public int ItemsPendingDeletion { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<ChartDataVM> DeletionTrend { get; set; } = new();
    public Dictionary<string, int> DeletedItemsByType { get; set; } = new();
    public Dictionary<string, int> DeletedItemsByReason { get; set; } = new();
    public List<DeletedContentSummaryVM> RecentDeletions { get; set; } = new();
}