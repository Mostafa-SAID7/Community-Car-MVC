namespace CommunityCar.Application.Features.Community.Guides.ViewModels;

public class GuideDetailVM
{
    public GuideVM Guide { get; set; } = new();
    public List<GuideVM> RelatedGuides { get; set; } = new();
    public List<GuideVM> AuthorOtherGuides { get; set; } = new();
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanVerify { get; set; }
    public bool CanFeature { get; set; }
}


