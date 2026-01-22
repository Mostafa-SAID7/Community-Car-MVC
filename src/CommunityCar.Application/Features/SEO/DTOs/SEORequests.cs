namespace CommunityCar.Application.Features.SEO.DTOs;

public class SEOMetricsRequest
{
    public int Views { get; set; }
    public int Clicks { get; set; }
    public double BounceRate { get; set; }
    public double AverageSessionDuration { get; set; }
}

public class ImageOptimizationOptions
{
    public int? MaxWidth { get; set; }
    public int? MaxHeight { get; set; }
    public int Quality { get; set; } = 80;
    public string Format { get; set; } = "webp";
    public bool PreserveAspectRatio { get; set; } = true;
    public bool GenerateResponsiveSizes { get; set; }
    public List<int> ResponsiveSizes { get; set; } = new();
}

public class CoreWebVitalsRequest
{
    public double LCP { get; set; }
    public double FID { get; set; }
    public double CLS { get; set; }
    public string UserAgent { get; set; } = string.Empty;
}

public class SitemapGenerationRequest
{
    public bool IncludeImages { get; set; }
    public bool IncludeVideos { get; set; }
}

public class RSSFeedRequest
{
    public string FeedType { get; set; } = "all";
    public int MaxItems { get; set; } = 50;
}