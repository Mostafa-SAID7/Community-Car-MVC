namespace CommunityCar.Web.Areas.Identity.ViewModels.Media;

/// <summary>
/// ViewModel for gallery images
/// </summary>
public class GalleryImageVM
{
    public Guid Id { get; set; }
    public Guid CollectionId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public List<string> Tags { get; set; } = new();
    public int SortOrder { get; set; }
    public DateTime UploadedDate { get; set; }
    public long FileSize { get; set; }
    public string? MimeType { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public bool IsLiked { get; set; }
    public bool IsCover { get; set; }
}
