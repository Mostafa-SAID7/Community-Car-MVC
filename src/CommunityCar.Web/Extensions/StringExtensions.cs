using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CommunityCar.Web.Extensions;

/// <summary>
/// Extension methods for string operations commonly used in web applications
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts a string to a URL-friendly slug
    /// </summary>
    public static string ToSlug(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Convert to lowercase
        var slug = input.ToLowerInvariant();

        // Remove diacritics (accents)
        slug = RemoveDiacritics(slug);

        // Replace spaces and invalid characters with hyphens
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");

        // Trim hyphens from start and end
        slug = slug.Trim('-');

        return slug;
    }

    /// <summary>
    /// Truncates a string to a specified length and adds ellipsis if needed
    /// </summary>
    public static string Truncate(this string input, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
            return input;

        return input.Substring(0, maxLength - suffix.Length) + suffix;
    }

    /// <summary>
    /// Truncates a string at word boundaries
    /// </summary>
    public static string TruncateAtWord(this string input, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
            return input;

        var truncated = input.Substring(0, maxLength - suffix.Length);
        var lastSpace = truncated.LastIndexOf(' ');
        
        if (lastSpace > 0)
            truncated = truncated.Substring(0, lastSpace);

        return truncated + suffix;
    }

    /// <summary>
    /// Converts string to title case
    /// </summary>
    public static string ToTitleCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }

    /// <summary>
    /// Converts string to camelCase
    /// </summary>
    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        if (input.Length == 1)
            return input.ToLower();

        return char.ToLower(input[0]) + input.Substring(1);
    }

    /// <summary>
    /// Converts string to PascalCase
    /// </summary>
    public static string ToPascalCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        if (input.Length == 1)
            return input.ToUpper();

        return char.ToUpper(input[0]) + input.Substring(1);
    }

    /// <summary>
    /// Checks if string is a valid email address
    /// </summary>
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Masks sensitive information in a string (like credit card numbers)
    /// </summary>
    public static string Mask(this string input, int visibleChars = 4, char maskChar = '*')
    {
        if (string.IsNullOrEmpty(input) || input.Length <= visibleChars)
            return input;

        var masked = new string(maskChar, input.Length - visibleChars);
        return masked + input.Substring(input.Length - visibleChars);
    }

    /// <summary>
    /// Removes HTML tags from a string
    /// </summary>
    public static string StripHtml(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return Regex.Replace(input, "<.*?>", string.Empty);
    }

    /// <summary>
    /// Converts newlines to HTML line breaks
    /// </summary>
    public static string NewLinesToBr(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return input.Replace("\r\n", "<br />").Replace("\n", "<br />").Replace("\r", "<br />");
    }

    /// <summary>
    /// Extracts numbers from a string
    /// </summary>
    public static string ExtractNumbers(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return Regex.Replace(input, @"[^\d]", "");
    }

    /// <summary>
    /// Checks if string contains only letters
    /// </summary>
    public static bool IsAlpha(this string input)
    {
        return !string.IsNullOrEmpty(input) && input.All(char.IsLetter);
    }

    /// <summary>
    /// Checks if string contains only numbers
    /// </summary>
    public static bool IsNumeric(this string input)
    {
        return !string.IsNullOrEmpty(input) && input.All(char.IsDigit);
    }

    /// <summary>
    /// Checks if string contains only letters and numbers
    /// </summary>
    public static bool IsAlphaNumeric(this string input)
    {
        return !string.IsNullOrEmpty(input) && input.All(char.IsLetterOrDigit);
    }

    /// <summary>
    /// Reverses a string
    /// </summary>
    public static string Reverse(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var chars = input.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    /// <summary>
    /// Removes diacritics (accents) from characters
    /// </summary>
    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Splits a string by comma and trims whitespace
    /// </summary>
    public static List<string> SplitAndTrim(this string input, char separator = ',')
    {
        if (string.IsNullOrEmpty(input))
            return new List<string>();

        return input.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                   .Select(s => s.Trim())
                   .Where(s => !string.IsNullOrEmpty(s))
                   .ToList();
    }
}