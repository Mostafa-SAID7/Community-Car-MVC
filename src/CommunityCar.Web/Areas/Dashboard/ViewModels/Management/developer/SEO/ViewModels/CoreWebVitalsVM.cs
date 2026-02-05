namespace CommunityCar.Application.Features.SEO.ViewModels;

public class CoreWebVitalsVM
{
    public decimal LCP { get; set; }
    public decimal FID { get; set; }
    public decimal CLS { get; set; }
    public decimal FCP { get; set; }
    public decimal TTI { get; set; }
    public decimal TBT { get; set; }
    public decimal SI { get; set; }
    public string LCPGrade { get; set; } = string.Empty;
    public string FIDGrade { get; set; } = string.Empty;
    public string CLSGrade { get; set; } = string.Empty;
}