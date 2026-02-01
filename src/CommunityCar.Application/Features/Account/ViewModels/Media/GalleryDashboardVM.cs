namespace CommunityCar.Application.Features.Account.ViewModels.Media;

public class GalleryDashboardVM
{
    public Guid UserId { get; set; }
    public int TotalItems { get; set; }
    public int PublicItems { get; set; }
    public int PrivateItems { get; set; }
    public long TotalStorageSize { get; set; }
    public string TotalStorageSizeFormatted { get; set; } = string.Empty;
    public int TotalViews { get; set; }
    public List<string> PopularTags { get; set; } = new();
    public List<UserGalleryItemVM> RecentItems { get; set; } = new();
    public List<UserGalleryItemVM> MostViewedItems { get; set; } = new();
    public List<UserGalleryItemVM> FeaturedItems { get; set; } = new();
}