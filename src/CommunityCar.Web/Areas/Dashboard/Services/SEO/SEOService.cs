using CommunityCar.Application.Common.Interfaces.Services.Dashboard.SEO;
using System.Text.Json;

namespace CommunityCar.Application.Services.Dashboard.SEO;

public class SEOService : ISEOService
{
    private readonly ILogger<SEOService> _logger;

    public SEOService(ILogger<SEOService> logger)
    {
        _logger = logger;
    }

    public async Task<Dictionary<string, object>> GenerateMetaDataAsync(string url)
    {
        // Mock implementation - generate basic meta data based on URL
        var metaData = new Dictionary<string, object>();

        if (url.Contains("/posts/"))
        {
            metaData["Title"] = "Community Car - Post Details";
            metaData["Description"] = "Read the latest community post on Community Car platform";
            metaData["Keywords"] = "community, car, automotive, post, discussion";
            metaData["OgType"] = "article";
        }
        else if (url.Contains("/qa/"))
        {
            metaData["Title"] = "Community Car - Q&A";
            metaData["Description"] = "Find answers to automotive questions on Community Car";
            metaData["Keywords"] = "community, car, automotive, questions, answers, qa";
            metaData["OgType"] = "article";
        }
        else if (url.Contains("/guides/"))
        {
            metaData["Title"] = "Community Car - Guides";
            metaData["Description"] = "Comprehensive automotive guides and tutorials";
            metaData["Keywords"] = "community, car, automotive, guides, tutorials, how-to";
            metaData["OgType"] = "article";
        }
        else
        {
            metaData["Title"] = "Community Car - Automotive Community Platform";
            metaData["Description"] = "Join the largest automotive community. Share experiences, ask questions, and connect with fellow car enthusiasts.";
            metaData["Keywords"] = "community, car, automotive, platform, social, networking";
            metaData["OgType"] = "website";
        }

        metaData["SiteName"] = "Community Car";
        metaData["TwitterCard"] = "summary_large_image";
        metaData["CanonicalUrl"] = $"https://communitycar.com{url}";
        metaData["OgTitle"] = metaData["Title"];
        metaData["OgDescription"] = metaData["Description"];

        return metaData;
    }

    public async Task<JsonDocument> GenerateStructuredDataAsync(string url)
    {
        // Mock implementation - generate basic structured data
        var structuredData = new
        {
            context = "https://schema.org",
            type = "WebSite",
            name = "Community Car",
            url = "https://communitycar.com",
            description = "Automotive community platform for car enthusiasts",
            potentialAction = new
            {
                type = "SearchAction",
                target = "https://communitycar.com/search?q={search_term_string}",
                queryInput = "required name=search_term_string"
            }
        };

        var json = JsonSerializer.Serialize(structuredData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        return JsonDocument.Parse(json);
    }

    public async Task<Dictionary<string, object>> AnalyzeSEOAsync(string url)
    {
        // Mock implementation
        return new Dictionary<string, object>
        {
            { "score", 85 },
            { "issues", new List<string> { "Missing alt text on some images", "Some pages lack meta descriptions" } },
            { "recommendations", new List<string> { "Add more internal links", "Optimize page loading speed" } },
            { "keywords", new List<string> { "community", "car", "automotive" } }
        };
    }

    public async Task<string> GenerateMetaDescriptionAsync(string content)
    {
        // Mock implementation - generate meta description from content
        if (string.IsNullOrEmpty(content))
            return "Community Car - Automotive Community Platform";

        var words = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var description = string.Join(" ", words.Take(20));
        
        if (description.Length > 160)
            description = description.Substring(0, 157) + "...";

        return description;
    }

    public async Task<string[]> GenerateKeywordsAsync(string content)
    {
        // Mock implementation - extract keywords from content
        if (string.IsNullOrEmpty(content))
            return new[] { "community", "car", "automotive" };

        var commonWords = new HashSet<string> { "the", "and", "or", "but", "in", "on", "at", "to", "for", "of", "with", "by", "a", "an", "is", "are", "was", "were" };
        var words = content.ToLower()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 3 && !commonWords.Contains(w))
            .GroupBy(w => w)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => g.Key)
            .ToArray();

        return words.Length > 0 ? words : new[] { "community", "car", "automotive" };
    }

    public async Task<int> GetSEOScoreAsync(string url)
    {
        // Mock implementation - calculate SEO score
        var score = 75; // Base score

        if (url.Contains("/posts/"))
            score += 5;
        if (url.Contains("/guides/"))
            score += 10;
        if (url.Contains("/qa/"))
            score += 8;

        return Math.Min(100, score);
    }
}