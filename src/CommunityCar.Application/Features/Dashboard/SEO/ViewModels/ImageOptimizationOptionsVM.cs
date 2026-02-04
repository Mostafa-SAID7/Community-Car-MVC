namespace CommunityCar.Application.Features.SEO.ViewModels;

public class ImageOptimizationOptionsVM
{
    public string Format { get; set; } = "webp";
    public int Quality { get; set; } = 85;
    public int MaxWidth { get; set; } = 1920;
    public int MaxHeight { get; set; } = 1080;
    public bool GenerateWebP { get; set; } = true;
    public bool GenerateAvif { get; set; } = false;
    public bool GenerateResponsiveSizes { get; set; } = true;
    public List<int> ResponsiveSizes { get; set; } = new() { 320, 640, 768, 1024, 1280, 1920 };
    public bool PreserveMetadata { get; set; } = false;
    public bool Progressive { get; set; } = true;
}