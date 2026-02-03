namespace CommunityCar.Application.Features.SEO.ViewModels;

public class SitemapGenerationVM
{
    public List<string> IncludeUrls { get; set; } = new();
    public List<string> ExcludeUrls { get; set; } = new();
    public string ChangeFrequency { get; set; } = "monthly";
    public double Priority { get; set; } = 0.5;
    public bool IncludeImages { get; set; } = true;
    public bool IncludeNews { get; set; } = false;
    public bool IncludeVideos { get; set; } = false;
}