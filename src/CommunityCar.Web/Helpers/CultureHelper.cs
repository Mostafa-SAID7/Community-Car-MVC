using System.Globalization;

namespace CommunityCar.Web.Helpers;

/// <summary>
/// Helper class for culture and localization operations
/// </summary>
public static class CultureHelper
{
    /// <summary>
    /// Supported cultures in the application
    /// </summary>
    public static readonly Dictionary<string, CultureInfo> SupportedCultures = new()
    {
        { "en-US", new CultureInfo("en-US") },
        { "ar-EG", new CultureInfo("ar-EG") }
    };

    /// <summary>
    /// Gets the current culture
    /// </summary>
    public static CultureInfo GetCurrentCulture()
    {
        return CultureInfo.CurrentCulture;
    }

    /// <summary>
    /// Gets the current UI culture
    /// </summary>
    public static CultureInfo GetCurrentUICulture()
    {
        return CultureInfo.CurrentUICulture;
    }

    /// <summary>
    /// Checks if the current culture is RTL (Right-to-Left)
    /// </summary>
    public static bool IsRightToLeft(string? culture = null)
    {
        var cultureToCheck = culture ?? GetCurrentCulture().Name;
        return cultureToCheck.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ||
               cultureToCheck.StartsWith("he", StringComparison.OrdinalIgnoreCase) ||
               cultureToCheck.StartsWith("fa", StringComparison.OrdinalIgnoreCase) ||
               cultureToCheck.StartsWith("ur", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the current culture is Arabic
    /// </summary>
    public static bool IsArabicCulture(string? culture = null)
    {
        var cultureToCheck = culture ?? GetCurrentCulture().Name;
        return cultureToCheck.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the direction attribute for HTML elements
    /// </summary>
    public static string GetDirection(string? culture = null)
    {
        return IsRightToLeft(culture) ? "rtl" : "ltr";
    }

    /// <summary>
    /// Gets the language code from culture
    /// </summary>
    public static string GetLanguageCode(string? culture = null)
    {
        var cultureToCheck = culture ?? GetCurrentCulture().Name;
        return cultureToCheck.Split('-')[0];
    }

    /// <summary>
    /// Gets the country code from culture
    /// </summary>
    public static string GetCountryCode(string? culture = null)
    {
        var cultureToCheck = culture ?? GetCurrentCulture().Name;
        var parts = cultureToCheck.Split('-');
        return parts.Length > 1 ? parts[1] : parts[0];
    }

    /// <summary>
    /// Formats a date according to the current culture
    /// </summary>
    public static string FormatDate(DateTime date, string? culture = null)
    {
        var cultureInfo = GetCultureInfo(culture);
        return date.ToString("d", cultureInfo);
    }

    /// <summary>
    /// Formats a date and time according to the current culture
    /// </summary>
    public static string FormatDateTime(DateTime dateTime, string? culture = null)
    {
        var cultureInfo = GetCultureInfo(culture);
        return dateTime.ToString("g", cultureInfo);
    }

    /// <summary>
    /// Formats a number according to the current culture
    /// </summary>
    public static string FormatNumber(decimal number, string? culture = null)
    {
        var cultureInfo = GetCultureInfo(culture);
        return number.ToString("N", cultureInfo);
    }

    /// <summary>
    /// Formats a currency according to the current culture
    /// </summary>
    public static string FormatCurrency(decimal amount, string? culture = null)
    {
        var cultureInfo = GetCultureInfo(culture);
        return amount.ToString("C", cultureInfo);
    }

    /// <summary>
    /// Gets the native name of a culture
    /// </summary>
    public static string GetNativeName(string culture)
    {
        if (SupportedCultures.TryGetValue(culture, out var cultureInfo))
        {
            return cultureInfo.NativeName;
        }
        return culture;
    }

    /// <summary>
    /// Gets the display name of a culture
    /// </summary>
    public static string GetDisplayName(string culture)
    {
        if (SupportedCultures.TryGetValue(culture, out var cultureInfo))
        {
            return cultureInfo.DisplayName;
        }
        return culture;
    }

    /// <summary>
    /// Validates if a culture is supported
    /// </summary>
    public static bool IsSupportedCulture(string culture)
    {
        return SupportedCultures.ContainsKey(culture);
    }

    /// <summary>
    /// Gets the default culture
    /// </summary>
    public static string GetDefaultCulture()
    {
        return "en-US";
    }

    /// <summary>
    /// Gets the fallback culture for a given culture
    /// </summary>
    public static string GetFallbackCulture(string culture)
    {
        if (culture.StartsWith("ar", StringComparison.OrdinalIgnoreCase))
            return "ar-EG";
        
        return "en-US";
    }

    /// <summary>
    /// Gets CultureInfo object for a given culture string
    /// </summary>
    public static CultureInfo GetCultureInfo(string? culture = null)
    {
        var cultureToUse = culture ?? GetCurrentCulture().Name;
        
        if (SupportedCultures.TryGetValue(cultureToUse, out var cultureInfo))
        {
            return cultureInfo;
        }

        // Try to get fallback culture
        var fallback = GetFallbackCulture(cultureToUse);
        if (SupportedCultures.TryGetValue(fallback, out var fallbackCulture))
        {
            return fallbackCulture;
        }

        // Return default culture
        return SupportedCultures[GetDefaultCulture()];
    }

    /// <summary>
    /// Gets all supported cultures for dropdown/selection
    /// </summary>
    public static Dictionary<string, string> GetSupportedCulturesForDisplay()
    {
        return SupportedCultures.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.NativeName
        );
    }

    /// <summary>
    /// Converts a relative time to culture-specific format
    /// </summary>
    public static string FormatRelativeTime(DateTime dateTime, string? culture = null)
    {
        var timeSpan = DateTime.UtcNow - dateTime;
        var cultureToCheck = culture ?? GetCurrentCulture().Name;
        var isArabic = IsArabicCulture(cultureToCheck);

        if (timeSpan.TotalSeconds < 60)
            return isArabic ? "الآن" : "just now";

        if (timeSpan.TotalMinutes < 60)
        {
            var minutes = (int)timeSpan.TotalMinutes;
            return isArabic ? $"منذ {minutes} دقيقة" : $"{minutes} minute{(minutes == 1 ? "" : "s")} ago";
        }

        if (timeSpan.TotalHours < 24)
        {
            var hours = (int)timeSpan.TotalHours;
            return isArabic ? $"منذ {hours} ساعة" : $"{hours} hour{(hours == 1 ? "" : "s")} ago";
        }

        if (timeSpan.TotalDays < 7)
        {
            var days = (int)timeSpan.TotalDays;
            return isArabic ? $"منذ {days} يوم" : $"{days} day{(days == 1 ? "" : "s")} ago";
        }

        if (timeSpan.TotalDays < 30)
        {
            var weeks = (int)(timeSpan.TotalDays / 7);
            return isArabic ? $"منذ {weeks} أسبوع" : $"{weeks} week{(weeks == 1 ? "" : "s")} ago";
        }

        if (timeSpan.TotalDays < 365)
        {
            var months = (int)(timeSpan.TotalDays / 30);
            return isArabic ? $"منذ {months} شهر" : $"{months} month{(months == 1 ? "" : "s")} ago";
        }

        var years = (int)(timeSpan.TotalDays / 365);
        return isArabic ? $"منذ {years} سنة" : $"{years} year{(years == 1 ? "" : "s")} ago";
    }

    /// <summary>
    /// Gets culture-specific CSS classes
    /// </summary>
    public static string GetCultureCssClasses(string? culture = null)
    {
        var classes = new List<string>();
        var cultureToCheck = culture ?? GetCurrentCulture().Name;

        classes.Add($"culture-{cultureToCheck.ToLower()}");
        classes.Add($"lang-{GetLanguageCode(cultureToCheck)}");
        
        if (IsRightToLeft(cultureToCheck))
        {
            classes.Add("rtl");
        }
        else
        {
            classes.Add("ltr");
        }

        if (IsArabicCulture(cultureToCheck))
        {
            classes.Add("arabic");
        }

        return string.Join(" ", classes);
    }

    /// <summary>
    /// Gets the appropriate number format for the culture
    /// </summary>
    public static string GetNumberFormat(string? culture = null)
    {
        var cultureInfo = GetCultureInfo(culture);
        return cultureInfo.NumberFormat.NumberDecimalSeparator == "," ? "european" : "american";
    }

    /// <summary>
    /// Gets the first day of week for the culture
    /// </summary>
    public static DayOfWeek GetFirstDayOfWeek(string? culture = null)
    {
        var cultureInfo = GetCultureInfo(culture);
        return cultureInfo.DateTimeFormat.FirstDayOfWeek;
    }

    /// <summary>
    /// Converts text direction based on culture
    /// </summary>
    public static string GetTextAlign(string? culture = null)
    {
        return IsRightToLeft(culture) ? "right" : "left";
    }

    /// <summary>
    /// Gets culture-specific font family
    /// </summary>
    public static string GetFontFamily(string? culture = null)
    {
        var cultureToCheck = culture ?? GetCurrentCulture().Name;
        
        if (IsArabicCulture(cultureToCheck))
        {
            return "'Noto Sans Arabic', 'Alexandria', sans-serif";
        }

        return "'Inter', 'Alexandria', system-ui, -apple-system, sans-serif";
    }
}