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
    /// Gets font preload links for the current culture
    /// </summary>
    /// <returns>HTML string with font preload links</returns>
    public static string GetFontPreloadLinks()
    {
        var links = new List<string>
        {
            "<link rel=\"preconnect\" href=\"https://fonts.googleapis.com\">",
            "<link rel=\"preconnect\" href=\"https://fonts.gstatic.com\" crossorigin>",
            "<link href=\"https://fonts.googleapis.com/css2?family=Alexandria:wght@100..900&display=swap\" rel=\"stylesheet\">",
            "<link href=\"https://fonts.googleapis.com/css2?family=Inter:wght@100;200;300;400;500;600;700;800;900&display=swap\" rel=\"stylesheet\">"
        };

        if (IsArabicCulture())
        {
            links.AddRange(new[]
            {
                "<link href=\"https://fonts.googleapis.com/css2?family=Lemonada:wght@300;400;500;600;700&display=swap\" rel=\"stylesheet\">",
                "<link href=\"https://fonts.googleapis.com/css2?family=Noto+Sans+Arabic:wght@100;200;300;400;500;600;700;800;900&display=swap\" rel=\"stylesheet\">"
            });
        }

        return string.Join("\n    ", links);
    }

    /// <summary>
    /// Gets inline CSS for font optimization
    /// </summary>
    /// <returns>CSS string for font optimization</returns>
    public static string GetFontOptimizationCSS()
    {
        var isArabic = IsArabicCulture();
        
        if (isArabic)
        {
            return @"
                <style>
                    /* Font display optimization for Arabic fonts */
                    @font-face {
                        font-family: 'Lemonada';
                        font-display: swap;
                    }
                    @font-face {
                        font-family: 'Noto Sans Arabic';
                        font-display: swap;
                    }
                    /* Prevent layout shift during font load */
                    body { font-family: 'Noto Sans Arabic', 'Alexandria', sans-serif; }
                    h1, h2, h3, h4, h5, h6 { font-family: 'Lemonada', 'Alexandria', cursive; }
                </style>";
        }
        
        return @"
            <style>
                /* Font display optimization for Latin fonts */
                @font-face {
                    font-family: 'Inter';
                    font-display: swap;
                }
                /* Prevent layout shift during font load */
                body { font-family: 'Inter', 'Alexandria', system-ui, -apple-system, sans-serif; }
            </style>";
    }
}