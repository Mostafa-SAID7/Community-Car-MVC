using CommunityCar.Application.Common.Interfaces.Services.Community.Moderation;
using System.Text.RegularExpressions;

namespace CommunityCar.Application.Services.Community.Moderation;

public class ContentModerationService : IContentModerationService
{
    // Basic list of toxic words - can be expanded as needed
    private readonly string[] _toxicWords = { 
        "badword1", "badword2", "toxicword", "offensive", "hate", "stupid", "idiot" 
    };

    public Task<string> FilterToxicContentAsync(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Task.FromResult(content);

        var filteredContent = content;
        foreach (var word in _toxicWords)
        {
            // Use regex for whole-word case-insensitive replacement
            var pattern = $@"\b{Regex.Escape(word)}\b";
            filteredContent = Regex.Replace(filteredContent, pattern, new string('*', word.Length), RegexOptions.IgnoreCase);
        }

        return Task.FromResult(filteredContent);
    }

    public Task<bool> IsContentToxicAsync(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Task.FromResult(false);

        foreach (var word in _toxicWords)
        {
            var pattern = $@"\b{Regex.Escape(word)}\b";
            if (Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase))
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}