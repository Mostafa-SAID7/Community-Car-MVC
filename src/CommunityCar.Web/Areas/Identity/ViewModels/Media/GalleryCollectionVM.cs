namespace CommunityCar.Web.Areas.Identity.ViewModels.Media;

public class GalleryCollectionVM
{
    public Guid UserId { get; set; }
    public List<UserGalleryItemVM> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasMore { get; set; }
    public string? CurrentTag { get; set; }
    public bool? IsPublicFilter { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}
