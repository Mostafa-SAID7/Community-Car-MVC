using CommunityCar.Application.Common.Interfaces.Repositories.User;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account.Profile;

/// <summary>
/// Service for profile validation operations
/// </summary>
public class ProfileValidationService : IProfileValidationService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProfileValidationService> _logger;

    public ProfileValidationService(
        IUserRepository userRepository,
        ILogger<ProfileValidationService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<bool> IsProfileCompleteAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            return !string.IsNullOrEmpty(user.FullName) &&
                   !string.IsNullOrEmpty(user.Bio) &&
                   !string.IsNullOrEmpty(user.City) &&
                   !string.IsNullOrEmpty(user.Country) &&
                   !string.IsNullOrEmpty(user.ProfilePictureUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking profile completion for user {UserId}", userId);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetProfileCompletionSuggestionsAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return Enumerable.Empty<string>();

            var suggestions = new List<string>();

            if (string.IsNullOrEmpty(user.Bio))
                suggestions.Add("Add a bio to tell others about yourself");

            if (string.IsNullOrEmpty(user.City) || string.IsNullOrEmpty(user.Country))
                suggestions.Add("Add your location");

            if (string.IsNullOrEmpty(user.ProfilePictureUrl))
                suggestions.Add("Upload a profile picture");

            if (string.IsNullOrEmpty(user.Website))
                suggestions.Add("Add your website or social media links");

            return suggestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile completion suggestions for user {UserId}", userId);
            return Enumerable.Empty<string>();
        }
    }

    public async Task<bool> IsUsernameAvailableAsync(string username, Guid? excludeUserId = null)
    {
        try
        {
            return await _userRepository.IsUserNameUniqueAsync(username, excludeUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking username availability for {Username}", username);
            return false;
        }
    }

    public async Task<bool> IsEmailAvailableAsync(string email, Guid? excludeUserId = null)
    {
        try
        {
            return await _userRepository.IsEmailUniqueAsync(email, excludeUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email availability for {Email}", email);
            return false;
        }
    }
}


