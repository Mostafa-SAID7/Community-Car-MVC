using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.QA.ViewModels;

public class CreateQuestionVM
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? BodyAr { get; set; }
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public string? CarEngine { get; set; }
    public string? Tags { get; set; }

    /// <summary>
    /// Helper property to get tags as a list
    /// </summary>
    public List<string> TagsList => 
        Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrEmpty(t))
            .ToList() ?? new List<string>();
}