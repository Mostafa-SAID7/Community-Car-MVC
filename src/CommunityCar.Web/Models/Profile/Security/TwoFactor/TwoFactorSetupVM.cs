using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Profile.Security.TwoFactor;

public class TwoFactorSetupVM
{
    public string SecretKey { get; set; } = string.Empty;
    public string QrCodeUrl { get; set; } = string.Empty;
    public string ManualEntryKey { get; set; } = string.Empty;
    public List<string> BackupCodes { get; set; } = new();
    
    [Required]
    [Display(Name = "Verification Code")]
    public string Code { get; set; } = string.Empty;
}