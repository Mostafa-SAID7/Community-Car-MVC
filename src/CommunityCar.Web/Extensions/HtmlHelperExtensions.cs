using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using CommunityCar.Web.Helpers;
using System.Text.Encodings.Web;

namespace CommunityCar.Web.Extensions;

/// <summary>
/// HTML Helper extensions for font and typography management
/// </summary>
public static class HtmlHelperExtensions
{
    /// <summary>
    /// Renders a heading with appropriate font styling based on culture
    /// </summary>
    /// <param name="htmlHelper">HTML helper instance</param>
    /// <param name="level">Heading level (1-6)</param>
    /// <param name="text">Heading text</param>
    /// <param name="htmlAttributes">Additional HTML attributes</param>
    /// <returns>HTML string for the heading</returns>
    public static IHtmlContent CultureHeading(this IHtmlHelper htmlHelper, int level, string text, object? htmlAttributes = null)
    {
        if (level < 1 || level > 6)
            throw new ArgumentException("Heading level must be between 1 and 6", nameof(level));

        var tagName = $"h{level}";
        var fontClass = FontHelper.GetTypographyClasses(tagName);
        
        var attributes = new Dictionary<string, object>();
        
        // Add font classes
        if (attributes.ContainsKey("class"))
        {
            attributes["class"] = $"{attributes["class"]} {fontClass}";
        }
        else
        {
            attributes["class"] = fontClass;
        }

        // Merge additional attributes
        if (htmlAttributes != null)
        {
            foreach (var prop in htmlAttributes.GetType().GetProperties())
            {
                var key = prop.Name.Replace("_", "-");
                var value = prop.GetValue(htmlAttributes);
                
                if (key == "class" && attributes.ContainsKey("class"))
                {
                    attributes["class"] = $"{attributes["class"]} {value}";
                }
                else
                {
                    attributes[key] = value;
                }
            }
        }

        var tag = new TagBuilder(tagName);
        tag.InnerHtml.SetContent(text);
        
        foreach (var attr in attributes)
        {
            tag.Attributes[attr.Key] = attr.Value?.ToString();
        }

        using var writer = new StringWriter();
        tag.WriteTo(writer, HtmlEncoder.Default);
        return new HtmlString(writer.ToString());
    }

    /// <summary>
    /// Renders a paragraph with appropriate font styling based on culture
    /// </summary>
    /// <param name="htmlHelper">HTML helper instance</param>
    /// <param name="text">Paragraph text</param>
    /// <param name="htmlAttributes">Additional HTML attributes</param>
    /// <returns>HTML string for the paragraph</returns>
    public static IHtmlContent CultureParagraph(this IHtmlHelper htmlHelper, string text, object? htmlAttributes = null)
    {
        var fontClass = FontHelper.GetTypographyClasses("p");
        
        var tag = new TagBuilder("p");
        tag.InnerHtml.SetContent(text);
        tag.AddCssClass(fontClass);

        if (htmlAttributes != null)
        {
            foreach (var prop in htmlAttributes.GetType().GetProperties())
            {
                var key = prop.Name.Replace("_", "-");
                var value = prop.GetValue(htmlAttributes);
                
                if (key == "class")
                {
                    tag.AddCssClass(value?.ToString());
                }
                else
                {
                    tag.Attributes[key] = value?.ToString();
                }
            }
        }

        using var writer = new StringWriter();
        tag.WriteTo(writer, HtmlEncoder.Default);
        return new HtmlString(writer.ToString());
    }

    /// <summary>
    /// Gets CSS classes for typography based on current culture
    /// </summary>
    /// <param name="htmlHelper">HTML helper instance</param>
    /// <param name="elementType">Type of element</param>
    /// <returns>CSS classes string</returns>
    public static string GetCultureFontClasses(this IHtmlHelper htmlHelper, string elementType)
    {
        return FontHelper.GetTypographyClasses(elementType);
    }

    /// <summary>
    /// Checks if current culture is Arabic
    /// </summary>
    /// <param name="htmlHelper">HTML helper instance</param>
    /// <returns>True if Arabic culture</returns>
    public static bool IsArabicCulture(this IHtmlHelper htmlHelper)
    {
        return FontHelper.IsArabicCulture();
    }

    /// <summary>
    /// Renders font preload links for current culture
    /// </summary>
    /// <param name="htmlHelper">HTML helper instance</param>
    /// <returns>HTML string with font preload links</returns>
    public static IHtmlContent FontPreloadLinks(this IHtmlHelper htmlHelper)
    {
        return new HtmlString(FontHelper.GetFontPreloadLinks());
    }

    /// <summary>
    /// Renders font optimization CSS for current culture
    /// </summary>
    /// <param name="htmlHelper">HTML helper instance</param>
    /// <returns>HTML string with optimization CSS</returns>
    public static IHtmlContent FontOptimizationCSS(this IHtmlHelper htmlHelper)
    {
        return new HtmlString(FontHelper.GetFontOptimizationCSS());
    }

    /// <summary>
    /// Renders a button with appropriate font styling based on culture
    /// </summary>
    /// <param name="htmlHelper">HTML helper instance</param>
    /// <param name="text">Button text</param>
    /// <param name="htmlAttributes">Additional HTML attributes</param>
    /// <returns>HTML string for the button</returns>
    public static IHtmlContent CultureButton(this IHtmlHelper htmlHelper, string text, object? htmlAttributes = null)
    {
        var fontClass = FontHelper.GetTypographyClasses("button");
        
        var tag = new TagBuilder("button");
        tag.InnerHtml.SetContent(text);
        tag.AddCssClass(fontClass);
        tag.Attributes["type"] = "button";

        if (htmlAttributes != null)
        {
            foreach (var prop in htmlAttributes.GetType().GetProperties())
            {
                var key = prop.Name.Replace("_", "-");
                var value = prop.GetValue(htmlAttributes);
                
                if (key == "class")
                {
                    tag.AddCssClass(value?.ToString());
                }
                else
                {
                    tag.Attributes[key] = value?.ToString();
                }
            }
        }

        using var writer = new StringWriter();
        tag.WriteTo(writer, HtmlEncoder.Default);
        return new HtmlString(writer.ToString());
    }

    /// <summary>
    /// Renders a label with appropriate font styling based on culture
    /// </summary>
    /// <param name="htmlHelper">HTML helper instance</param>
    /// <param name="text">Label text</param>
    /// <param name="htmlAttributes">Additional HTML attributes</param>
    /// <returns>HTML string for the label</returns>
    public static IHtmlContent CultureLabel(this IHtmlHelper htmlHelper, string text, object? htmlAttributes = null)
    {
        var fontClass = FontHelper.GetTypographyClasses("label");
        
        var tag = new TagBuilder("label");
        tag.InnerHtml.SetContent(text);
        tag.AddCssClass(fontClass);

        if (htmlAttributes != null)
        {
            foreach (var prop in htmlAttributes.GetType().GetProperties())
            {
                var key = prop.Name.Replace("_", "-");
                var value = prop.GetValue(htmlAttributes);
                
                if (key == "class")
                {
                    tag.AddCssClass(value?.ToString());
                }
                else
                {
                    tag.Attributes[key] = value?.ToString();
                }
            }
        }

        using var writer = new StringWriter();
        tag.WriteTo(writer, HtmlEncoder.Default);
        return new HtmlString(writer.ToString());
    }
}