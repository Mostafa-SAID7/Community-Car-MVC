using CommunityCar.Application.Common.Interfaces.Services.Dashboard.SEO;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Services.Dashboard.SEO;

public class SEOService : ISEOService
{
    public async Task<SEOAnalysisVM> AnalyzeSEOAsync(string url)
    {
        await Task.CompletedTask;

        var random = new Random();
        var score = random.Next(60, 95);

        return new SEOAnalysisVM
        {
            Url = url,
            OverallScore = score,
            AnalyzedAt = DateTime.UtcNow,
            Title = "Community Car - Car Sharing Platform",
            MetaDescription = "Join our community-driven car sharing platform. Find, share, and rent cars in your neighborhood.",
            TitleLength = 45,
            MetaDescriptionLength = 95,
            HasH1Tag = true,
            H1Count = 1,
            H1Text = "Welcome to Community Car",
            HasMetaKeywords = false,
            MetaKeywords = "",
            HasCanonicalUrl = true,
            CanonicalUrl = url,
            HasOpenGraphTags = true,
            HasTwitterCardTags = true,
            HasStructuredData = random.Next(2) == 0,
            ImageCount = random.Next(5, 20),
            ImagesWithoutAlt = random.Next(0, 5),
            InternalLinksCount = random.Next(10, 50),
            ExternalLinksCount = random.Next(5, 20),
            PageLoadTime = (decimal)(random.NextDouble() * 3 + 1), // 1-4 seconds
            PageSize = random.Next(500000, 2000000), // 500KB - 2MB
            IsMobileFriendly = random.Next(10) > 1, // 90% mobile friendly
            HasSitemap = true,
            HasRobotsTxt = true,
            SslEnabled = true,
            Issues = GenerateSEOIssues(score),
            Recommendations = GenerateSEORecommendations(score),
            Keywords = new List<SEOKeywordVM>
            {
                new() { Keyword = "car sharing", Position = random.Next(1, 10), SearchVolume = 12000, Difficulty = "Medium" },
                new() { Keyword = "community car", Position = random.Next(1, 15), SearchVolume = 3400, Difficulty = "Low" },
                new() { Keyword = "rent car", Position = random.Next(5, 20), SearchVolume = 45000, Difficulty = "High" }
            }
        };
    }

    public async Task<List<SEOKeywordVM>> GetKeywordRankingsAsync(string domain)
    {
        await Task.CompletedTask;

        var keywords = new List<SEOKeywordVM>();
        var sampleKeywords = new[]
        {
            "car sharing", "community car", "rent car", "car rental", "vehicle sharing",
            "peer to peer car", "car booking", "shared mobility", "urban transport", "eco friendly transport"
        };

        var random = new Random();

        foreach (var keyword in sampleKeywords)
        {
            keywords.Add(new SEOKeywordVM
            {
                Keyword = keyword,
                Position = random.Next(1, 100),
                PreviousPosition = random.Next(1, 100),
                SearchVolume = random.Next(1000, 50000),
                Difficulty = new[] { "Low", "Medium", "High" }[random.Next(3)],
                Cpc = (decimal)(random.NextDouble() * 5 + 0.5), // $0.5 - $5.5
                Competition = (decimal)(random.NextDouble()),
                Trend = random.Next(3) switch
                {
                    0 => "Up",
                    1 => "Down",
                    _ => "Stable"
                },
                Url = $"https://communitycar.com/{keyword.Replace(" ", "-")}",
                LastUpdated = DateTime.UtcNow.AddDays(-random.Next(1, 7))
            });
        }

        return keywords.OrderBy(k => k.Position).ToList();
    }

    public async Task<SEOReportVM> GenerateSEOReportAsync(string domain)
    {
        var analysis = await AnalyzeSEOAsync($"https://{domain}");
        var keywords = await GetKeywordRankingsAsync(domain);

        return new SEOReportVM
        {
            Domain = domain,
            GeneratedAt = DateTime.UtcNow,
            OverallScore = analysis.OverallScore,
            Analysis = analysis,
            Keywords = keywords,
            TopKeywords = keywords.Where(k => k.Position <= 10).ToList(),
            ImprovingKeywords = keywords.Where(k => k.Trend == "Up").ToList(),
            DecliningKeywords = keywords.Where(k => k.Trend == "Down").ToList(),
            Recommendations = analysis.Recommendations,
            CompetitorAnalysis = await GetCompetitorAnalysisAsync(domain)
        };
    }

    public async Task<bool> UpdateSEOSettingsAsync(SEOSettingsVM settings)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<SEOSettingsVM> GetSEOSettingsAsync()
    {
        return new SEOSettingsVM
        {
            DefaultTitle = "Community Car - Car Sharing Platform",
            DefaultMetaDescription = "Join our community-driven car sharing platform. Find, share, and rent cars in your neighborhood.",
            DefaultMetaKeywords = "car sharing, community car, rent car, vehicle sharing, peer to peer",
            SiteName = "Community Car",
            DefaultOgImage = "/images/og-default.jpg",
            TwitterHandle = "@communitycar",
            GoogleAnalyticsId = "GA-XXXXXXXXX",
            GoogleSearchConsoleId = "GSC-XXXXXXXXX",
            BingWebmasterToolsId = "",
            EnableSitemap = true,
            SitemapUrl = "/sitemap.xml",
            EnableRobotsTxt = true,
            RobotsTxtContent = "User-agent: *\nAllow: /\nSitemap: https://communitycar.com/sitemap.xml",
            EnableCanonicalUrls = true,
            EnableOpenGraph = true,
            EnableTwitterCards = true,
            EnableStructuredData = true,
            DefaultLanguage = "en",
            EnableHreflang = false,
            EnableAmpPages = false,
            TrackingCodes = new Dictionary<string, string>
            {
                { "Google Analytics", "GA-XXXXXXXXX" },
                { "Facebook Pixel", "FB-XXXXXXXXX" },
                { "Hotjar", "HJ-XXXXXXXXX" }
            }
        };
    }

    public async Task<List<SEOIssueVM>> GetSEOIssuesAsync(string? url = null)
    {
        await Task.CompletedTask;

        var issues = new List<SEOIssueVM>();
        var random = new Random();
        var severities = new[] { "Low", "Medium", "High", "Critical" };
        var categories = new[] { "Technical", "Content", "Performance", "Mobile", "Security" };

        var sampleIssues = new[]
        {
            "Missing meta description on 5 pages",
            "Duplicate title tags found",
            "Images without alt text",
            "Slow page load time",
            "Missing H1 tags",
            "Broken internal links",
            "Non-mobile friendly pages",
            "Missing canonical URLs",
            "Large image files",
            "Missing structured data"
        };

        for (int i = 0; i < random.Next(3, 8); i++)
        {
            var issue = sampleIssues[random.Next(sampleIssues.Length)];
            issues.Add(new SEOIssueVM
            {
                Id = Guid.NewGuid(),
                Title = issue,
                Description = $"Detailed description of the issue: {issue}",
                Severity = severities[random.Next(severities.Length)],
                Category = categories[random.Next(categories.Length)],
                Url = url ?? $"https://communitycar.com/page{random.Next(1, 10)}",
                DetectedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                IsFixed = random.Next(4) == 0, // 25% fixed
                FixedAt = null,
                Impact = random.Next(1, 10),
                Recommendation = $"Fix recommendation for: {issue}",
                Priority = random.Next(1, 5)
            });
        }

        return issues.OrderByDescending(i => i.Priority).ToList();
    }

    public async Task<List<CompetitorAnalysisVM>> GetCompetitorAnalysisAsync(string domain)
    {
        await Task.CompletedTask;

        var competitors = new List<CompetitorAnalysisVM>();
        var competitorDomains = new[] { "zipcar.com", "turo.com", "getaround.com", "car2go.com" };
        var random = new Random();

        foreach (var competitor in competitorDomains)
        {
            competitors.Add(new CompetitorAnalysisVM
            {
                Domain = competitor,
                SEOScore = random.Next(70, 95),
                OrganicTraffic = random.Next(100000, 1000000),
                KeywordCount = random.Next(5000, 50000),
                BacklinkCount = random.Next(10000, 100000),
                DomainAuthority = random.Next(60, 90),
                TopKeywords = new List<string> { "car sharing", "rent car", "vehicle rental" },
                TrafficTrend = random.Next(3) switch
                {
                    0 => "Up",
                    1 => "Down",
                    _ => "Stable"
                },
                LastAnalyzed = DateTime.UtcNow.AddDays(-random.Next(1, 7))
            });
        }

        return competitors.OrderByDescending(c => c.SEOScore).ToList();
    }

    public async Task<bool> SubmitSitemapAsync(string sitemapUrl)
    {
        // In real implementation, submit sitemap to search engines
        await Task.Delay(1000);
        return true;
    }

    public async Task<List<ChartDataVM>> GetSEOMetricsChartAsync(string metricType, int days = 30)
    {
        var data = new List<ChartDataVM>();
        var random = new Random();
        var startDate = DateTime.UtcNow.AddDays(-days);

        for (int i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i);
            decimal value = metricType.ToLower() switch
            {
                "organic-traffic" => random.Next(1000, 10000),
                "keyword-rankings" => random.Next(50, 500),
                "backlinks" => random.Next(10, 100),
                "seo-score" => random.Next(70, 95),
                _ => random.Next(0, 100)
            };

            data.Add(new ChartDataVM
            {
                Label = date.ToString("MMM dd"),
                Value = value,
                Date = date
            });
        }

        return await Task.FromResult(data);
    }

    private List<SEOIssueVM> GenerateSEOIssues(int score)
    {
        var issues = new List<SEOIssueVM>();
        var random = new Random();

        if (score < 80)
        {
            issues.Add(new SEOIssueVM
            {
                Id = Guid.NewGuid(),
                Title = "Page load time is slow",
                Description = "Page takes more than 3 seconds to load",
                Severity = "High",
                Category = "Performance",
                Impact = 8,
                Recommendation = "Optimize images and enable compression"
            });
        }

        if (score < 70)
        {
            issues.Add(new SEOIssueVM
            {
                Id = Guid.NewGuid(),
                Title = "Missing meta descriptions",
                Description = "Several pages are missing meta descriptions",
                Severity = "Medium",
                Category = "Content",
                Impact = 6,
                Recommendation = "Add unique meta descriptions to all pages"
            });
        }

        return issues;
    }

    private List<SEORecommendationVM> GenerateSEORecommendations(int score)
    {
        var recommendations = new List<SEORecommendationVM>();

        recommendations.Add(new SEORecommendationVM
        {
            Title = "Optimize page titles",
            Description = "Ensure all pages have unique, descriptive titles under 60 characters",
            Priority = "High",
            EstimatedImpact = "High",
            Effort = "Low"
        });

        if (score < 85)
        {
            recommendations.Add(new SEORecommendationVM
            {
                Title = "Improve page load speed",
                Description = "Optimize images, enable compression, and minimize CSS/JS files",
                Priority = "High",
                EstimatedImpact = "High",
                Effort = "Medium"
            });
        }

        return recommendations;
    }
}