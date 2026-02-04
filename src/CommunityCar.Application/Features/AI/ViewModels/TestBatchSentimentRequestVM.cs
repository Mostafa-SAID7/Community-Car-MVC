using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

public class TestBatchSentimentRequestVM
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public List<string> Texts { get; set; } = new();
}