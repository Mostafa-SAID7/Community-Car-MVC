using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.SoftDelete.ViewModels;

/// <summary>
/// Statistics for soft deleted content
/// </summary>
public class SoftDeleteStatsVM
{
    public int DeletedPosts { get; set; }
    public int DeletedStories { get; set; }
    public int DeletedGroups { get; set; }
    public int DeletedComments { get; set; }
    public int TotalDeleted { get; set; }
    public DateTime LastCleanup { get; set; }
    public int ItemsCleanedUpLastTime { get; set; }
}
