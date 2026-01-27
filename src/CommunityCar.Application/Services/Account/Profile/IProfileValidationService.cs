namespace CommunityCar.Application.Services.Account.Profile;

/// <summary>
/// Interface for profile validation operations
/// </summary>
public interface IProfileValidationService
{
    Task<bool> IsProfileCompleteAsync(Guid userId);
    Task<IEnumerable<string>> GetProfileCompletionSuggestionsAsync(Guid userId);
    Task<bool> IsUsernameAvailableAsync(string username, Guid? excludeUserId = null);
    Task<bool> IsEmailAvailableAsync(string email, Guid? excludeUserId = null);
}


