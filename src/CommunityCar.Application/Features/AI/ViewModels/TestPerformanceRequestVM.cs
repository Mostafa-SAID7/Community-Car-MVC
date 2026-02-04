using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

public class TestPerformanceRequestVM
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string TestMessage { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Context { get; set; }

    [Range(1, 100)]
    public int Iterations { get; set; } = 10;
}