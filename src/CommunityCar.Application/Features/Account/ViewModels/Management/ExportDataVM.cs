using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class ExportDataVM
{
    [Display(Name = "Include Profile Data")]
    public bool IncludeProfile { get; set; } = true;

    [Display(Name = "Include Activity Logs")]
    public bool IncludeActivity { get; set; } = true;

    [Display(Name = "Include Media")]
    public bool IncludeMedia { get; set; } = false;
}