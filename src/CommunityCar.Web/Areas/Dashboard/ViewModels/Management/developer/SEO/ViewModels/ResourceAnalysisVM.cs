namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.developer.SEO.ViewModels;

public class ResourceAnalysisVM
{
    public int TotalRequests { get; set; }
    public long TotalSize { get; set; }
    public int JavaScriptFiles { get; set; }
    public long JavaScriptSize { get; set; }
    public int CSSFiles { get; set; }
    public long CSSSize { get; set; }
    public int ImageFiles { get; set; }
    public long ImageSize { get; set; }
    public int FontFiles { get; set; }
    public long FontSize { get; set; }
    public List<string> RenderBlockingResources { get; set; } = new();
    public List<string> UnusedResources { get; set; } = new();
}




