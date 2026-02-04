using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

public class TestSuggestionsRequestVM
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;

    [Range(1, 20)]
    public int MaxSuggestions { get; set; } = 5;
}