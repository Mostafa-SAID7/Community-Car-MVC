using System.Threading.Tasks;

namespace CommunityCar.Application.Common.Interfaces.Services.Community.Moderation;

public interface IContentModerationService
{
    /// <summary>
    /// Filters toxic content by replacing bad words with asterisks.
    /// </summary>
    Task<string> FilterToxicContentAsync(string content);

    /// <summary>
    /// Checks if the content contains any toxic or forbidden words.
    /// </summary>
    Task<bool> IsContentToxicAsync(string content);
}



