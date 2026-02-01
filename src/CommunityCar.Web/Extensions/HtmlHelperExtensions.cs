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
                    attributes[key] = value?.ToString() ?? string.Empty;
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
                    var cssClass = value?.ToString();
                    if (!string.IsNullOrEmpty(cssClass))
                        tag.AddCssClass(cssClass);
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


    public static string GetCultureFontClasses(this IHtmlHelper htmlHelper, string elementType)
    {
        return FontHelper.GetTypographyClasses(elementType);
    }


    public static bool IsArabicCulture(this IHtmlHelper htmlHelper)
    {
        return FontHelper.IsArabicCulture();
    }


    public static IHtmlContent FontPreloadLinks(this IHtmlHelper htmlHelper)
    {
        return new HtmlString(FontHelper.GetFontPreloadLinks());
    }


    public static IHtmlContent FontOptimizationCSS(this IHtmlHelper htmlHelper)
    {
        return new HtmlString(FontHelper.GetFontOptimizationCSS());
    }

  
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
                    var cssClass = value?.ToString();
                    if (!string.IsNullOrEmpty(cssClass))
                        tag.AddCssClass(cssClass);
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
                    var cssClass = value?.ToString();
                    if (!string.IsNullOrEmpty(cssClass))
                    {
                        tag.AddCssClass(cssClass);
                    }
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


