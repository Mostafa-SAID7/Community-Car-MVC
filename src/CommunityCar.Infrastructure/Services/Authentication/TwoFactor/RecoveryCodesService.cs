using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CommunityCar.Infrastructure.Services.Authentication.TwoFactor;

/// <summary>
/// Service for recovery codes management
/// </summary>
public class RecoveryCodesService : IRecoveryCodesService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RecoveryCodesService> _logger;

    public RecoveryCodesService(
        UserManager<User> userManager,
        ILogger<RecoveryCodesService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            // Generate recovery codes
            var recoveryCodes = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var code = GenerateRecoveryCode();
                recoveryCodes.Add(code);
            }

            // Store recovery codes (hashed)
            var hashedCodes = recoveryCodes.Select(HashRecoveryCode).ToList();
            user.BackupCodes = JsonSerializer.Serialize(hashedCodes);
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Recovery codes generated for user {UserId}", request.UserId);
            
            return Result.Success("Recovery codes generated successfully.", new { RecoveryCodes = recoveryCodes });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating recovery codes for user {UserId}", request.UserId);
            return Result.Failure("An error occurred while generating recovery codes.");
        }
    }

    public async Task<Result> VerifyRecoveryCodeAsync(VerifyRecoveryCodeRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            if (string.IsNullOrEmpty(user.BackupCodes))
            {
                return Result.Failure("No recovery codes available.");
            }

            var hashedCodes = JsonSerializer.Deserialize<List<string>>(user.BackupCodes) ?? new List<string>();
            var hashedInputCode = HashRecoveryCode(request.RecoveryCode);

            if (hashedCodes.Contains(hashedInputCode))
            {
                // Remove used code
                hashedCodes.Remove(hashedInputCode);
                user.BackupCodes = JsonSerializer.Serialize(hashedCodes);
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Recovery code verified and used for user {UserId}", request.UserId);
                return Result.Success("Recovery code verified successfully.");
            }

            _logger.LogWarning("Invalid recovery code for user {UserId}", request.UserId);
            return Result.Failure("Invalid recovery code.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying recovery code for user {UserId}", request.UserId);
            return Result.Failure("An error occurred while verifying the recovery code.");
        }
    }

    public async Task<int> GetRecoveryCodesCountAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.BackupCodes))
            {
                return 0;
            }

            var hashedCodes = JsonSerializer.Deserialize<List<string>>(user.BackupCodes) ?? new List<string>();
            return hashedCodes.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recovery codes count for user {UserId}", userId);
            return 0;
        }
    }

    private string GenerateRecoveryCode()
    {
        var bytes = new byte[4];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return BitConverter.ToUInt32(bytes, 0).ToString("D8");
    }

    private string HashRecoveryCode(string code)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(code));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
