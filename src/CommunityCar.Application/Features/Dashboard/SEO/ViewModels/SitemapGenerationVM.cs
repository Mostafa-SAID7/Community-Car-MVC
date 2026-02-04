namespace CommunityCar.Application.Features.Dashboard.SEO.ViewModels;

public class SitemapGenerationVM
{
    public bool IncludePages { get; set; } = true;
    public bool IncludePosts { get; set; } = true;
    public bool IncludeCategories { get; set; } = true;
    public bool IncludeUsers { get; set; } = false;
    public DateTime? LastModified { get; set; }
    public string ChangeFrequency { get; set; } = "weekly";
    public double Priority { get; set; } = 0.5;
    public List<string> ExcludeUrls { get; set; } = new();
}