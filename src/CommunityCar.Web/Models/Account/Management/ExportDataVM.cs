using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Account.Management;

public class ExportDataVM
{
    [Display(Name = "Include Profile Information")]
    public bool IncludeProfile { get; set; } = true;

    [Display(Name = "Include Posts")]
    public bool IncludePosts { get; set; } = true;

    [Display(Name = "Include Comments")]
    public bool IncludeComments { get; set; } = true;

    [Display(Name = "Include Messages")]
    public bool IncludeMessages { get; set; } = true;

    [Display(Name = "Include Activity Log")]
    public bool IncludeActivity { get; set; } = false;
}