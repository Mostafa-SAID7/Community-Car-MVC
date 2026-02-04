using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.News.ViewModels;

public class CreateNewsVM
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Summary { get; set; } = string.Empty;
    
    public string? FeaturedImageUrl { get; set; }
    public List<string> Tags { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public DateTime? PublishDate { get; set; }
    public bool AllowComments { get; set; } = true;
    public Dictionary<string, object> Metadata { get; set; } = new();
}