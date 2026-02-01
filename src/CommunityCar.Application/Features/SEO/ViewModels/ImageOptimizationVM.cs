namespace CommunityCar.Application.Features.SEO.ViewModels;

public class ImageOptimizationVM
{
    public string OriginalPath { get; set; } = string.Empty;
    public string OptimizedPath { get; set; } = string.Empty;
    public long OriginalSize { get; set; }
    public long OptimizedSize { get; set; }
    public decimal CompressionRatio { get; set; }
    public string Format { get; set; } = string.Empty;
    public int? Width { get; set; }
    public int? Height { get; set; }
    public bool HasWebPVersion { get; set; }
    public bool HasAvifVersion { get; set; }
    public List<string> GeneratedSizes { get; set; } = new();
    public double SavingsPercentage { get; set; }
}