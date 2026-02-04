using CommunityCar.Application.Common.Interfaces.Services.Account.Gamification;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Caching;
using CommunityCar.Application.Configuration.Caching;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Dashboard.BackgroundJobs;

/// <summary>
/// Background job service for gamification operations
/// </summary>
public class GamificationBackgroundJobService
{
    private readonly IGamificationService _gamificationService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GamificationBackgroundJobService> _logger;

    public GamificationBackgroundJobService(
        IGamificationService gamificationService,
        ICacheService cacheService,
        ILogger<GamificationBackgroundJobService> logger)
    {
        _gamificationService = gamificationService;
        _cacheService = cacheService;
        _logger = logger;
    }

    /// <summary>
    /// Process badge awards for user actions
    /// </summary>
    public async Task ProcessBadgeAwardsAsync(Guid userId, string actionType, Dictionary<string, object>? actionData = null)
    {
        try
        {
            _logger.LogInformation("Processing badge awards for user {UserId}, action: {ActionType}", userId, actionType);
            
            await _gamificationService.GetGamificationAsync();
            
            // Invalidate user's gamification cache
            await _cacheService.RemoveByPatternAsync(CacheKeys.Patterns.GamificationData(userId));
            
            _logger.LogInformation("Badge awards processed successfully for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process badge awards for user {UserId}, action: {ActionType}", userId, actionType);
            throw;
        }
    }

    /// <summary>
    /// Update user points and levels
    /// </summary>
    public async Task UpdateUserPointsAsync(Guid userId, int points, string reason)
    {
        try
        {
            _logger.LogInformation("Updating points for user {UserId}: {Points} points for {Reason}", userId, points, reason);
            
            await _gamificationService.GetGamificationAsync();
            
            // Invalidate user's points and level cache
            await _cacheService.RemoveAsync(CacheKeys.Gamification.UserPoints(userId));
            await _cacheService.RemoveAsync(CacheKeys.Gamification.UserLevel(userId));
            
            _logger.LogInformation("Points updated successfully for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update points for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Check and award achievements
    /// </summary>
    public async Task CheckAchievementsAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation("Checking achievements for user {UserId}", userId);
            
            await _gamificationService.GetGamificationAsync();
            
            // Invalidate user's achievement cache
            await _cacheService.RemoveAsync(CacheKeys.Profile.Achievements(userId));
            await _cacheService.RemoveAsync(CacheKeys.Gamification.AchievementProgress(userId));
            
            _logger.LogInformation("Achievement check completed for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check achievements for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Update leaderboards
    /// </summary>
    public async Task UpdateLeaderboardsAsync()
    {
        try
        {
            _logger.LogInformation("Updating leaderboards");
            
            await _gamificationService.GetGamificationAsync();
            
            // Invalidate leaderboard cache
            await _cacheService.RemoveByPatternAsync($"{CacheKeys.Gamification.Leaderboard("*", 0).Split(':')[0]}:*");
            
            _logger.LogInformation("Leaderboards updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update leaderboards");
            throw;
        }
    }

    /// <summary>
    /// Process daily gamification tasks
    /// </summary>
    public async Task ProcessDailyTasksAsync()
    {
        try
        {
            _logger.LogInformation("Processing daily gamification tasks");
            
            // Reset daily challenges
            await _gamificationService.GetGamificationAsync();
            
            // Update weekly/monthly progress
            await _gamificationService.GetGamificationAsync();
            
            // Clean up expired achievements
            await _gamificationService.GetGamificationAsync();
            
            _logger.LogInformation("Daily gamification tasks completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process daily gamification tasks");
            throw;
        }
    }

    /// <summary>
    /// Batch process user actions for multiple users
    /// </summary>
    public async Task BatchProcessUserActionsAsync(List<(Guid UserId, string ActionType, Dictionary<string, object>? ActionData)> userActions)
    {
        try
        {
            _logger.LogInformation("Batch processing {Count} user actions", userActions.Count);
            
            var tasks = userActions.Select(async action =>
            {
                try
                {
                    await ProcessBadgeAwardsAsync(action.UserId, action.ActionType, action.ActionData);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process action for user {UserId}", action.UserId);
                }
            });

            await Task.WhenAll(tasks);
            
            _logger.LogInformation("Batch processing completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to batch process user actions");
            throw;
        }
    }
}


