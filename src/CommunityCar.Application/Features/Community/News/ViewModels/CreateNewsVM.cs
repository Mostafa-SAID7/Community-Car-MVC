namespace CommunityCar.Application.Features.Community.News.ViewModels;

public class CreateNewsVM
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public bool IsPublished { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public DateTime? PublishDate { get; set; }
}

public class EditNewsVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public bool IsPublished { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public DateTime? PublishDate { get; set; }
}