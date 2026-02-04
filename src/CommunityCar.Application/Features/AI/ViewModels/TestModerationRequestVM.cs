using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

public class TestModerationRequestVM
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
}