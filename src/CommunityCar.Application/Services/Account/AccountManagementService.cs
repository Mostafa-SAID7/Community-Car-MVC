using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Text.Json;

namespace CommunityCar.Application.Services.Account;

/// <summary>
/// Unified service for account management operations (lifecycle, privacy, data export)
/// </summary>
public class AccountManagementService : IAccountManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<Domain.Entities.Auth.User> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AccountManagementService> _logger;

    public AccountManagementService(
        IUserRepository userRepository,
        UserManager<Domain.Entities.Auth.User> userManager,
        ICurrentUserService currentUserService,
        ILogger<AccountManagementService> logger)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Account Lifecycle

    public async Task<Result> DeactivateAccountAsync(DeactivateAccountRequest request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null) return Result.Failure("User not found.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid) return Result.Failure("Invalid password.");

            var result = await _userRepository.DeactivateUserAsync(request.UserId, request.Reason);
            if (result)
            {
                _logger.LogInformation("Account deactivated for user {UserId}", request.UserId);
                return Result.Success("Account deactivated successfully.");
            }
            return Result.Failure("Failed to deactivate account.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating account for user {UserId}", request.UserId);
            return Result.Failure("An error occurred while deactivating account.");
        }
    }

    public async Task<Result> ReactivateAccountAsync(Guid userId, string password)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return Result.Failure("User not found.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid) return Result.Failure("Invalid password.");

            var result = await _userRepository.ReactivateUserAsync(userId);
            if (result)
            {
                _logger.LogInformation("Account reactivated for user {UserId}", userId);
                return Result.Success("Account reactivated successfully.");
            }
            return Result.Failure("Failed to reactivate account.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reactivating account for user {UserId}", userId);
            return Result.Failure("An error occurred while reactivating account.");
        }
    }

    public async Task<Result> DeleteAccountAsync(DeleteAccountRequest request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null) return Result.Failure("User not found.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid) return Result.Failure("Invalid password.");

            user.Delete();
            user.Audit(_currentUserService.UserId);
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Account deleted for user {UserId}", request.UserId);
            return Result.Success("Account deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account for user {UserId}", request.UserId);
            return Result.Failure("An error occurred while deleting account.");
        }
    }

    public async Task<bool> IsAccountDeactivatedAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null && !user.IsActive && !user.IsDeleted;
    }

    public async Task<bool> CanRecoverAccountAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user != null && !user.IsActive && !user.IsDeleted;
    }

    public async Task<Result> RecoverAccountAsync(string email, string password)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return Result.Failure("User not found.");
            if (user.IsActive) return Result.Failure("Account is already active.");
            if (user.IsDeleted) return Result.Failure("Account cannot be recovered.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid) return Result.Failure("Invalid password.");

            var result = await _userRepository.ReactivateUserAsync(user.Id);
            if (result)
            {
                _logger.LogInformation("Account recovered for user {UserId}", user.Id);
                return Result.Success("Account recovered successfully.");
            }
            return Result.Failure("Failed to recover account.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recovering account for email {Email}", email);
            return Result.Failure("An error occurred while recovering account.");
        }
    }

    #endregion

    #region Privacy & Consent

    public async Task<PrivacySettingsVM> GetPrivacySettingsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return new PrivacySettingsVM();

        return new PrivacySettingsVM
        {
            ProfileVisibility = user.IsPublic ? "Public" : "Private",
            ShowEmail = user.ShowEmail,
            ShowLocation = user.ShowLocation,
            ShowOnlineStatus = user.ShowOnlineStatus,
            AllowMessagesFromStrangers = user.AllowMessagesFromStrangers,
            AllowTagging = user.AllowTagging,
            ShowActivityStatus = user.ShowActivityStatus,
            DataProcessingConsent = user.DataProcessingConsent,
            MarketingEmailsConsent = user.MarketingEmailsConsent,
            LastUpdated = user.UpdatedAt ?? DateTime.UtcNow
        };
    }

    public async Task<Result> UpdatePrivacySettingsAsync(UpdatePrivacySettingsRequest request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null) return Result.Failure("User not found.");

            user.IsPublic = request.ProfileVisible;
            user.ShowEmail = request.EmailVisible;
            user.ShowLocation = request.PhoneVisible;
            user.ShowOnlineStatus = request.ShowOnlineStatus;
            user.AllowMessagesFromStrangers = request.AllowMessages;
            user.AllowTagging = request.AllowTagging;
            user.ShowActivityStatus = request.ShowActivityStatus;
            user.DataProcessingConsent = request.AllowFriendRequests;
            user.MarketingEmailsConsent = request.AllowTagging;
            user.Audit(_currentUserService.UserId);

            await _userRepository.UpdateAsync(user);
            return Result.Success("Privacy settings updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating privacy settings for user {UserId}", request.UserId);
            return Result.Failure("An error occurred while updating privacy settings.");
        }
    }

    public async Task<bool> AcceptTermsOfServiceAsync(Guid userId, string version)
    {
        _logger.LogInformation("Terms of service accepted for user {UserId}, version {Version}", userId, version);
        return true;
    }

    public async Task<bool> AcceptPrivacyPolicyAsync(Guid userId, string version)
    {
        _logger.LogInformation("Privacy policy accepted for user {UserId}, version {Version}", userId, version);
        return true;
    }

    public async Task<IEnumerable<ConsentVM>> GetUserConsentsAsync(Guid userId)
    {
        return new List<ConsentVM>();
    }

    #endregion

    #region Data Export

    public async Task<Result> ExportUserDataAsync(ExportUserDataRequest request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null) return Result.Failure("User not found.");

            var exportData = new Dictionary<string, object>();
            if (request.IncludeProfile)
            {
                exportData["profile"] = new { user.Id, user.FullName, user.Email, user.Bio, user.City, user.Country, user.Website, user.CreatedAt, user.LastLoginAt };
            }

            var jsonData = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
            var zipData = CreateZipFile(jsonData);

            return Result.Success("Data export completed successfully.", zipData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting data for user {UserId}", request.UserId);
            return Result.Failure("An error occurred while exporting data.");
        }
    }

    public async Task<bool> RequestDataExportAsync(Guid userId)
    {
        _logger.LogInformation("Data export requested for user {UserId}", userId);
        return true;
    }

    public async Task<IEnumerable<DataExportVM>> GetDataExportHistoryAsync(Guid userId)
    {
        return new List<DataExportVM>();
    }

    private static byte[] CreateZipFile(string jsonData)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var entry = archive.CreateEntry("user-data.json");
            using var entryStream = entry.Open();
            using var writer = new StreamWriter(entryStream);
            writer.Write(jsonData);
        }
        return memoryStream.ToArray();
    }

    #endregion

    #region Account Information

    public async Task<AccountInfoVM> GetAccountInfoAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return new AccountInfoVM();

        return new AccountInfoVM
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive,
            IsDeactivated = !user.IsActive && !user.IsDeleted,
            DeactivatedAt = !user.IsActive ? user.UpdatedAt : null
        };
    }

    public async Task<DateTime> GetAccountCreationDateAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.CreatedAt ?? DateTime.MinValue;
    }

    public async Task<DateTime?> GetLastLoginDateAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.LastLoginAt;
    }

    #endregion
}


