using System.Globalization;

namespace CommunityCar.Web.Middleware;

/// <summary>
/// Middleware for handling culture and localization
/// </summary>
public class CultureMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CultureMiddleware> _logger;
    private readonly CultureOptions _options;

    public CultureMiddleware(RequestDelegate next, ILogger<CultureMiddleware> logger, CultureOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var culture = DetermineCulture(context);
        
        if (!string.IsNullOrEmpty(culture))
        {
            try
            {
                var cultureInfo = new CultureInfo(culture);
                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;
                
                // Store culture in HttpContext for later use
                context.Items["Culture"] = culture;
                context.Items["CultureInfo"] = cultureInfo;
                
                _logger.LogDebug("Set culture to {Culture} for request {Path}", culture, context.Request.Path);
            }
            catch (CultureNotFoundException ex)
            {
                _logger.LogWarning(ex, "Invalid culture {Culture} requested, falling back to default", culture);
                SetDefaultCulture(context);
            }
        }
        else
        {
            SetDefaultCulture(context);
        }

        await _next(context);
    }

    private string? DetermineCulture(HttpContext context)
    {
        // 1. Check URL path (highest priority)
        var cultureFromPath = GetCultureFromPath(context);
        if (!string.IsNullOrEmpty(cultureFromPath) && _options.SupportedCultures.Contains(cultureFromPath))
        {
            return cultureFromPath;
        }

        // 2. Check query string
        if (_options.EnableQueryStringCulture)
        {
            var cultureFromQuery = context.Request.Query["culture"].FirstOrDefault();
            if (!string.IsNullOrEmpty(cultureFromQuery) && _options.SupportedCultures.Contains(cultureFromQuery))
            {
                return cultureFromQuery;
            }
        }

        // 3. Check cookie
        if (_options.EnableCookieCulture)
        {
            var cultureFromCookie = context.Request.Cookies[_options.CultureCookieName];
            if (!string.IsNullOrEmpty(cultureFromCookie) && _options.SupportedCultures.Contains(cultureFromCookie))
            {
                return cultureFromCookie;
            }
        }

        // 4. Check user preferences (if authenticated)
        if (_options.EnableUserPreferenceCulture && context.User?.Identity?.IsAuthenticated == true)
        {
            var cultureFromUser = GetUserPreferredCulture(context);
            if (!string.IsNullOrEmpty(cultureFromUser) && _options.SupportedCultures.Contains(cultureFromUser))
            {
                return cultureFromUser;
            }
        }

        // 5. Check Accept-Language header
        if (_options.EnableAcceptLanguageHeader)
        {
            var cultureFromHeader = GetCultureFromAcceptLanguage(context);
            if (!string.IsNullOrEmpty(cultureFromHeader) && _options.SupportedCultures.Contains(cultureFromHeader))
            {
                return cultureFromHeader;
            }
        }

        // 6. Fall back to default
        return _options.DefaultCulture;
    }

    private string? GetCultureFromPath(HttpContext context)
    {
        var path = context.Request.Path.Value;
        if (string.IsNullOrEmpty(path) || path == "/")
        {
            return null;
        }

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 0)
        {
            var firstSegment = segments[0];
            
            // Check if first segment is a supported culture
            if (_options.SupportedCultures.Contains(firstSegment))
            {
                return firstSegment;
            }
        }

        return null;
    }

    private string? GetUserPreferredCulture(HttpContext context)
    {
        // This would typically come from user profile/settings
        // For now, return null - implement based on your user management system
        var userCultureClaim = context.User?.FindFirst("culture")?.Value;
        return userCultureClaim;
    }

    private string? GetCultureFromAcceptLanguage(HttpContext context)
    {
        var acceptLanguage = context.Request.Headers.AcceptLanguage.FirstOrDefault();
        if (string.IsNullOrEmpty(acceptLanguage))
        {
            return null;
        }

        // Parse Accept-Language header and find best match
        var languages = acceptLanguage
            .Split(',')
            .Select(lang => lang.Split(';')[0].Trim())
            .Where(lang => !string.IsNullOrEmpty(lang));

        foreach (var language in languages)
        {
            // Try exact match first
            if (_options.SupportedCultures.Contains(language))
            {
                return language;
            }

            // Try language part only (e.g., "en" from "en-US")
            var languagePart = language.Split('-')[0];
            var matchingCulture = _options.SupportedCultures.FirstOrDefault(c => c.StartsWith(languagePart + "-"));
            if (!string.IsNullOrEmpty(matchingCulture))
            {
                return matchingCulture;
            }
        }

        return null;
    }

    private void SetDefaultCulture(HttpContext context)
    {
        try
        {
            var defaultCultureInfo = new CultureInfo(_options.DefaultCulture);
            CultureInfo.CurrentCulture = defaultCultureInfo;
            CultureInfo.CurrentUICulture = defaultCultureInfo;
            
            context.Items["Culture"] = _options.DefaultCulture;
            context.Items["CultureInfo"] = defaultCultureInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set default culture {DefaultCulture}", _options.DefaultCulture);
        }
    }
}

/// <summary>
/// Configuration options for culture middleware
/// </summary>
public class CultureOptions
{
    public string DefaultCulture { get; set; } = "en-US";
    public List<string> SupportedCultures { get; set; } = new() { "en-US", "ar-EG" };
    public bool EnableQueryStringCulture { get; set; } = true;
    public bool EnableCookieCulture { get; set; } = true;
    public bool EnableUserPreferenceCulture { get; set; } = true;
    public bool EnableAcceptLanguageHeader { get; set; } = true;
    public string CultureCookieName { get; set; } = "Culture";
    public TimeSpan CultureCookieExpiration { get; set; } = TimeSpan.FromDays(30);
}