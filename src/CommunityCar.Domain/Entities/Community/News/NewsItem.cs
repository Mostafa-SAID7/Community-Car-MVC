using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Domain.Entities.Community.News;

public class NewsItem : BaseEntity
{
    public string Headline { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public string? Summary { get; private set; }
    
    // Arabic Localization
    public string? HeadlineAr { get; private set; }
    public string? BodyAr { get; private set; }
    public string? SummaryAr { get; private set; }
    
    public string? ImageUrl { get; private set; }
    public List<string> ImageUrls { get; private set; } = new();
    public NewsCategory Category { get; private set; }
    public string? CarMake { get; private set; }
    public string? CarModel { get; private set; }
    public int? CarYear { get; private set; }
    public List<string> Tags { get; private set; } = new();
    public string? Source { get; private set; }
    public string? SourceUrl { get; private set; }
    public Guid AuthorId { get; private set; }
    public bool IsPublished { get; private set; }
    public bool IsFeatured { get; private set; }
    public bool IsPinned { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public int ViewCount { get; private set; }
    public int LikeCount { get; private set; }
    public int CommentCount { get; private set; }
    public int ShareCount { get; private set; }
    public string? MetaTitle { get; private set; }
    public string? MetaDescription { get; private set; }
    public string Slug { get; private set; } = string.Empty;

    // Computed properties
    public string? CarDisplayName => 
        !string.IsNullOrWhiteSpace(CarMake) && !string.IsNullOrWhiteSpace(CarModel) 
            ? $"{CarMake} {CarModel}" + (CarYear.HasValue ? $" ({CarYear})" : "")
            : !string.IsNullOrWhiteSpace(CarMake) 
                ? CarMake 
                : null;

    // Private constructor for EF Core
    private NewsItem() { }

    public NewsItem(string headline, string body, Guid authorId, NewsCategory category)
    {
        Headline = headline;
        Body = body;
        AuthorId = authorId;
        Category = category;
        Slug = GenerateSlug(headline);
        IsPublished = false;
        IsFeatured = false;
        IsPinned = false;
        ViewCount = 0;
        LikeCount = 0;
        CommentCount = 0;
        ShareCount = 0;
    }

    public void UpdateContent(string headline, string body, string? summary = null)
    {
        Headline = headline;
        Body = body;
        Summary = summary;
        Slug = GenerateSlug(headline);
        Audit(AuthorId.ToString());
    }

    public void UpdateArabicContent(string? headlineAr, string? bodyAr, string? summaryAr)
    {
        HeadlineAr = headlineAr;
        BodyAr = bodyAr;
        SummaryAr = summaryAr;
        Audit(AuthorId.ToString());
    }

    public void UpdateCategory(NewsCategory category)
    {
        Category = category;
        Audit(AuthorId.ToString());
    }

    public void SetMainImage(string imageUrl)
    {
        ImageUrl = imageUrl;
        Audit(AuthorId.ToString());
    }

    public void AddImage(string imageUrl)
    {
        if (!ImageUrls.Contains(imageUrl))
        {
            ImageUrls.Add(imageUrl);
            Audit(AuthorId.ToString());
        }
    }

    public void RemoveImage(string imageUrl)
    {
        if (ImageUrls.Remove(imageUrl))
        {
            Audit(AuthorId.ToString());
        }
    }

    public void SetCarInfo(string? carMake, string? carModel, int? carYear)
    {
        CarMake = carMake;
        CarModel = carModel;
        CarYear = carYear;
        Audit(AuthorId.ToString());
    }

    public void AddTag(string tag)
    {
        var normalizedTag = tag.Trim().ToLowerInvariant();
        if (!Tags.Any(t => t.ToLowerInvariant() == normalizedTag))
        {
            Tags.Add(tag.Trim());
            Audit(AuthorId.ToString());
        }
    }

    public void RemoveTag(string tag)
    {
        var normalizedTag = tag.Trim().ToLowerInvariant();
        var existingTag = Tags.FirstOrDefault(t => t.ToLowerInvariant() == normalizedTag);
        if (existingTag != null && Tags.Remove(existingTag))
        {
            Audit(AuthorId.ToString());
        }
    }

    public void ClearTags()
    {
        if (Tags.Any())
        {
            Tags.Clear();
            Audit(AuthorId.ToString());
        }
    }

    public void SetSource(string? source, string? sourceUrl)
    {
        Source = source;
        SourceUrl = sourceUrl;
        Audit(AuthorId.ToString());
    }

    public void Publish()
    {
        if (!IsPublished)
        {
            IsPublished = true;
            PublishedAt = DateTime.UtcNow;
            Audit(AuthorId.ToString());
        }
    }

    public void Unpublish()
    {
        if (IsPublished)
        {
            IsPublished = false;
            PublishedAt = null;
            Audit(AuthorId.ToString());
        }
    }

    public void SetFeatured(bool featured)
    {
        if (IsFeatured != featured)
        {
            IsFeatured = featured;
            Audit(AuthorId.ToString());
        }
    }

    public void SetPinned(bool pinned)
    {
        if (IsPinned != pinned)
        {
            IsPinned = pinned;
            Audit(AuthorId.ToString());
        }
    }

    public void IncrementViewCount()
    {
        ViewCount++;
        Audit(AuthorId.ToString());
    }

    public void IncrementLikeCount()
    {
        LikeCount++;
        Audit(AuthorId.ToString());
    }

    public void DecrementLikeCount()
    {
        if (LikeCount > 0)
        {
            LikeCount--;
            Audit(AuthorId.ToString());
        }
    }

    public void IncrementCommentCount()
    {
        CommentCount++;
        Audit(AuthorId.ToString());
    }

    public void DecrementCommentCount()
    {
        if (CommentCount > 0)
        {
            CommentCount--;
            Audit(AuthorId.ToString());
        }
    }

    public void IncrementShareCount()
    {
        ShareCount++;
        Audit(AuthorId.ToString());
    }

    public void UpdateSeoData(string? metaTitle, string? metaDescription)
    {
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
        Audit(AuthorId.ToString());
    }

    private static string GenerateSlug(string headline)
    {
        if (string.IsNullOrWhiteSpace(headline))
            return Guid.NewGuid().ToString("N")[..8];

        var slug = headline.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace("/", "-")
            .Replace("\\", "-")
            .Replace("&", "and");

        // Remove multiple consecutive dashes
        while (slug.Contains("--"))
            slug = slug.Replace("--", "-");

        // Remove leading and trailing dashes
        slug = slug.Trim('-');

        // Ensure slug is not empty and not too long
        if (string.IsNullOrWhiteSpace(slug))
            slug = Guid.NewGuid().ToString("N")[..8];
        else if (slug.Length > 100)
            slug = slug[..100].TrimEnd('-');

        return slug;
    }
}