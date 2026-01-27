using System.Globalization;

namespace CommunityCar.Web.Helpers;

/// <summary>
/// Helper class for managing font styles based on current culture
/// </summary>
public static class FontHelper
{
    /// <summary>
    /// Gets the appropriate heading font class based on current culture
    /// </summary>
    /// <returns>CSS class string for headings</returns>
    public static string GetHeadingFontClass()
    {
        return IsArabicCulture() ? "font-arabic-heading" : "font-sans";
    }

    /// <summary>
    /// Gets the appropriate body font class based on current culture
    /// </summary>
    /// <returns>CSS class string for body text</returns>
    public static string GetBodyFontClass()
    {
        return IsArabicCulture() ? "font-arabic-body" : "font-sans";
    }

    /// <summary>
    /// Gets the appropriate font family CSS value for headings
    /// </summary>
    /// <returns>CSS font-family value</returns>
    public static string GetHeadingFontFamily()
    {
        return IsArabicCulture() 
            ? "'Lemonada', 'Alexandria', cursive" 
            : "'Inter', 'Alexandria', system-ui, -apple-system, sans-serif";
    }

    /// <summary>
    /// Gets the appropriate font family CSS value for body text
    /// </summary>
    /// <returns>CSS font-family value</returns>
    public static string GetBodyFontFamily()
    {
        return IsArabicCulture() 
            ? "'Noto Sans Arabic', 'Alexandria', sans-serif" 
            : "'Inter', 'Alexandria', system-ui, -apple-system, sans-serif";
    }

    /// <summary>
    /// Gets the appropriate line height for the current culture
    /// </summary>
    /// <param name="textType">Type of text (heading, body, small)</param>
    /// <returns>CSS line-height value</returns>
    public static string GetLineHeight(string textType = "body")
    {
        if (!IsArabicCulture()) return "1.5";

        return textType.ToLower() switch
        {
            "heading" => "1.6",
            "body" => "1.9",
            "small" => "1.7",
            "xs" => "1.6",
            _ => "1.8"
        };
    }

    /// <summary>
    /// Gets CSS classes for typography based on element type and culture
    /// </summary>
    /// <param name="elementType">Type of element (h1, h2, p, span, etc.)</param>
    /// <returns>CSS classes string</returns>
    public static string GetTypographyClasses(string elementType)
    {
        var isArabic = IsArabicCulture();
        
        return elementType.ToLower() switch
        {
            "h1" => isArabic ? "font-arabic-heading font-semibold leading-tight" : "font-sans font-bold",
            "h2" => isArabic ? "font-arabic-heading font-semibold leading-tight" : "font-sans font-bold",
            "h3" => isArabic ? "font-arabic-heading font-semibold leading-tight" : "font-sans font-semibold",
            "h4" => isArabic ? "font-arabic-heading font-medium leading-snug" : "font-sans font-semibold",
            "h5" => isArabic ? "font-arabic-heading font-medium leading-snug" : "font-sans font-medium",
            "h6" => isArabic ? "font-arabic-heading font-medium leading-snug" : "font-sans font-medium",
            "p" => isArabic ? "font-arabic-body leading-relaxed" : "font-sans leading-relaxed",
            "span" => isArabic ? "font-arabic-body" : "font-sans",
            "button" => isArabic ? "font-arabic-body font-medium" : "font-sans font-medium",
            "input" => isArabic ? "font-arabic-body" : "font-sans",
            "label" => isArabic ? "font-arabic-body font-medium" : "font-sans font-medium",
            _ => isArabic ? "font-arabic-body" : "font-sans"
        };
    }

    /// <summary>
    /// Checks if the current culture is Arabic
    /// </summary>
    /// <returns>True if current culture is Arabic</returns>
    public static bool IsArabicCulture()
    {
        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("ar", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets font preload links for performance optimization
    /// </summary>
    /// <returns>HTML string with font preload links</returns>
    public static string GetFontPreloadLinks()
    {
        if (IsArabicCulture())
        {
            return @"
                <link rel=""preload"" href=""/fonts/NotoSansArabic-Regular.woff2"" as=""font"" type=""font/woff2"" crossorigin>
                <link rel=""preload"" href=""/fonts/Lemonada-Regular.woff2"" as=""font"" type=""font/woff2"" crossorigin>";
        }
        else
        {
            return @"
                <link rel=""preload"" href=""/fonts/Inter-Regular.woff2"" as=""font"" type=""font/woff2"" crossorigin>
                <link rel=""preload"" href=""/fonts/Inter-Bold.woff2"" as=""font"" type=""font/woff2"" crossorigin>";
        }
    }

    /// <summary>
    /// Gets font optimization CSS for performance
    /// </summary>
    /// <returns>CSS string for font optimization</returns>
    public static string GetFontOptimizationCSS()
    {
        return @"
            .font-arabic-heading, .font-arabic-body {
                font-display: swap;
            }
            .font-sans {
                font-display: swap;
            }";
    }
}



