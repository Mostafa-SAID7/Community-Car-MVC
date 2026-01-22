using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CommunityCar.Web.TagHelpers;

[HtmlTargetElement("img")]
[HtmlTargetElement("iframe")]
public class LazyLoadingTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // Automatically add loading="lazy" if not already present
        if (!output.Attributes.ContainsName("loading"))
        {
            output.Attributes.Add("loading", "lazy");
        }

        // For images, ensure decodings="async" for better performance (helps with LCP)
        if (output.TagName == "img" && !output.Attributes.ContainsName("decoding"))
        {
            output.Attributes.Add("decoding", "async");
        }
        
        // Ensure width and height are provided to avoid layout shifts (helps with CLS)
        // This is a recommendation, we can't easily force it without knowing the image size
        // but we can add a warning in development mode if they are missing.
    }
}
