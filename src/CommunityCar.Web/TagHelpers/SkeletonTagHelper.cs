using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CommunityCar.Web.TagHelpers;

[HtmlTargetElement("skeleton")]
public class SkeletonTagHelper : TagHelper
{
    private readonly IHtmlHelper _htmlHelper;

    public SkeletonTagHelper(IHtmlHelper htmlHelper)
    {
        _htmlHelper = htmlHelper;
    }

    [HtmlAttributeName("type")]
    public string Type { get; set; } = "Post";

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        (_htmlHelper as IViewContextAware)!.Contextualize(ViewContext);
        
        var content = await _htmlHelper.PartialAsync("_SkeletonLoading", Type);
        
        output.TagName = null; // Remove the <skeleton> tag itself
        output.Content.SetHtmlContent(content);
    }
}



