namespace CommunityCar.Web.Areas.Identity.ViewModels.Media;

public class GalleryTagsVM
{
    public Guid UserId { get; set; }
    public List<TagUsageVM> Tags { get; set; } = new();
    public int TotalTags { get; set; }
}
