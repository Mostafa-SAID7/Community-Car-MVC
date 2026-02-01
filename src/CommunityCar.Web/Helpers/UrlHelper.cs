using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Web;

namespace CommunityCar.Web.Helpers;

/// <summary>
/// Helper class for URL generation and manipulation
/// </summary>
public static class UrlHelper
{
    /// <summary>
    /// Generates a culture-aware URL
    /// </summary>
    public static string CultureUrl(IUrlHelper urlHelper, string action, string controller, string culture, object? routeValues = null)
    {
        var values = new Dictionary<string, object?> { { "culture", culture } };
        
        if (routeValues != null)
        {
            foreach (var property in routeValues.GetType().GetProperties())
            {
                values[property.Name] = property.GetValue(routeValues);
            }
        }

        return urlHelper.Action(action, controller, values) ?? "#";
    }

    /// <summary>
    /// Generates a profile URL with slug
    /// </summary>
    public static string ProfileUrl(IUrlHelper urlHelper, string culture, string? slug = null, Guid? userId = null)
    {
        if (!string.IsNullOrEmpty(slug))
        {
            return urlHelper.Action("ViewProfileBySlug", "Profile", new { culture, slug }) ?? "#";
        }
        
        if (userId.HasValue)
        {
            return urlHelper.Action("ViewProfile", "Profile", new { culture, id = userId }) ?? "#";
        }

        return urlHelper.Action("Index", "Profile", new { culture }) ?? "#";
    }

    /// <summary>
    /// Generates a paginated URL
    /// </summary>
    public static string PaginatedUrl(IUrlHelper urlHelper, string action, string controller, int page, object? routeValues = null)
    {
        var values = new Dictionary<string, object?> { { "page", page } };
        
        if (routeValues != null)
        {
            foreach (var property in routeValues.GetType().GetProperties())
            {
                values[property.Name] = property.GetValue(routeValues);
            }
        }

        return urlHelper.Action(action, controller, values) ?? "#";
    }

    /// <summary>
    /// Generates a search URL with query parameters
    /// </summary>
    public static string SearchUrl(IUrlHelper urlHelper, string action, string controller, string query, string? category = null, object? filters = null)
    {
        var values = new Dictionary<string, object?> 
        { 
            { "q", query },
            { "category", category }
        };
        
        if (filters != null)
        {
            foreach (var property in filters.GetType().GetProperties())
            {
                values[property.Name] = property.GetValue(filters);
            }
        }

        return urlHelper.Action(action, controller, values) ?? "#";
    }

    /// <summary>
    /// Generates an API URL
    /// </summary>
    public static string ApiUrl(IUrlHelper urlHelper, string action, string controller, object? routeValues = null)
    {
        var values = new Dictionary<string, object?>();
        
        if (routeValues != null)
        {
            foreach (var property in routeValues.GetType().GetProperties())
            {
                values[property.Name] = property.GetValue(routeValues);
            }
        }

        return urlHelper.Action(action, controller, values, "https") ?? "#";
    }

    /// <summary>
    /// Generates a URL with query string parameters
    /// </summary>
    public static string WithQueryString(string baseUrl, Dictionary<string, object?> parameters)
    {
        if (!parameters.Any()) return baseUrl;

        var queryString = string.Join("&", 
            parameters.Where(p => p.Value != null)
                     .Select(p => $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(p.Value?.ToString())}"));

        var separator = baseUrl.Contains('?') ? "&" : "?";
        return $"{baseUrl}{separator}{queryString}";
    }

    /// <summary>
    /// Extracts query parameters from URL
    /// </summary>
    public static Dictionary<string, string> GetQueryParameters(string url)
    {
        var parameters = new Dictionary<string, string>();
        
        var queryIndex = url.IndexOf('?');
        if (queryIndex == -1) return parameters;

        var queryString = url.Substring(queryIndex + 1);
        var pairs = queryString.Split('&');

        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=');
            if (keyValue.Length == 2)
            {
                var key = HttpUtility.UrlDecode(keyValue[0]);
                var value = HttpUtility.UrlDecode(keyValue[1]);
                parameters[key] = value;
            }
        }

        return parameters;
    }

    /// <summary>
    /// Generates a friendly URL slug from text
    /// </summary>
    public static string GenerateSlug(string text, int maxLength = 50)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        // Convert to lowercase and remove diacritics
        var slug = text.ToLowerInvariant();
        slug = RemoveDiacritics(slug);

        // Replace invalid characters with hyphens
        var sb = new StringBuilder();
        foreach (var c in slug)
        {
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(c);
            }
            else if (char.IsWhiteSpace(c) || c == '-' || c == '_')
            {
                if (sb.Length > 0 && sb[sb.Length - 1] != '-')
                {
                    sb.Append('-');
                }
            }
        }

        slug = sb.ToString().Trim('-');
        
        // Truncate if too long
        if (slug.Length > maxLength)
        {
            slug = slug.Substring(0, maxLength).TrimEnd('-');
        }

        return slug;
    }

    /// <summary>
    /// Validates if a URL is safe for redirection
    /// </summary>
    public static bool IsSafeUrl(string url, string host)
    {
        if (string.IsNullOrEmpty(url)) return false;
        
        // Check for absolute URLs
        if (Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri))
        {
            return string.Equals(absoluteUri.Host, host, StringComparison.OrdinalIgnoreCase);
        }

        // Check for relative URLs
        if (Uri.TryCreate(url, UriKind.Relative, out _))
        {
            return !url.StartsWith("//") && !url.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    /// <summary>
    /// Combines URL paths safely
    /// </summary>
    public static string CombinePaths(params string[] paths)
    {
        if (paths == null || paths.Length == 0) return string.Empty;

        var result = paths[0]?.TrimEnd('/') ?? string.Empty;
        
        for (int i = 1; i < paths.Length; i++)
        {
            var path = paths[i]?.Trim('/') ?? string.Empty;
            if (!string.IsNullOrEmpty(path))
            {
                result = $"{result}/{path}";
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the base URL from a full URL
    /// </summary>
    public static string GetBaseUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return $"{uri.Scheme}://{uri.Host}{(uri.IsDefaultPort ? "" : $":{uri.Port}")}";
        }

        return string.Empty;
    }

    /// <summary>
    /// Removes diacritics from text for URL generation
    /// </summary>
    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
    }

    /// <summary>
    /// Generates a return URL for authentication scenarios
    /// </summary>
    public static string GenerateReturnUrl(IUrlHelper urlHelper, string? returnUrl, string defaultAction, string defaultController)
    {
        if (!string.IsNullOrEmpty(returnUrl) && urlHelper.IsLocalUrl(returnUrl))
        {
            return returnUrl;
        }

        return urlHelper.Action(defaultAction, defaultController) ?? "/";
    }

    /// <summary>
    /// Generates a canonical URL for SEO
    /// </summary>
    public static string GenerateCanonicalUrl(string baseUrl, string path)
    {
        var cleanPath = path.TrimStart('/');
        var cleanBaseUrl = baseUrl.TrimEnd('/');
        return $"{cleanBaseUrl}/{cleanPath}";
    }
}