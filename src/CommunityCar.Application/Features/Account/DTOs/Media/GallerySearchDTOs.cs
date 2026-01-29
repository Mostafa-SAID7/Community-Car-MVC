namespace CommunityCar.Application.Features.Account.DTOs.Media;

#region Gallery Search DTOs

public class GallerySearchRequest
{
    public Guid UserId { get; set; }
    public string? Tag { get; set; }
    public bool? IsPublic { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "CreatedAt"; // CreatedAt, ViewCount, DisplayOrder
    public bool SortDescending { get; set; } = true;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GallerySearchResult
{
    public List<UserGalleryDTO> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasMore { get; set; }
    public string? SearchTag { get; set; }
}

public class GalleryFilterRequest
{
    public Guid UserId { get; set; }
    public List<string>? Tags { get; set; }
    public bool? IsPublic { get; set; }
    public long? MinFileSize { get; set; }
    public long? MaxFileSize { get; set; }
    public List<string>? MimeTypes { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}

#endregion