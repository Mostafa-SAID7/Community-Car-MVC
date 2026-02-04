using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

public class TopContentVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public double EngagementRate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastViewedAt { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public int Rank { get; set; }
    public double Score { get; set; }
}