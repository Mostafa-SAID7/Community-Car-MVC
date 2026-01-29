using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Authorization;
using CommunityCar.Domain.Constants;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account;

public class ProgressionService : IProgressionService
{
    private readonly IGuidesRepository _guidesRepository;
    private readonly IReviewsRepository _reviewsRepository;
    private readonly INewsRepository _newsRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleService _roleService;
    private readonly ILogger<ProgressionService> _logger;

    public ProgressionService(
        IGuidesRepository guidesRepository,
        IReviewsRepository reviewsRepository,
        INewsRepository newsRepository,
        IUserRepository userRepository,
        IRoleService roleService,
        ILogger<ProgressionService> logger)
    {
        _guidesRepository = guidesRepository;
        _reviewsRepository = reviewsRepository;
        _newsRepository = newsRepository;
        _userRepository = userRepository;
        _roleService = roleService;
        _logger = logger;
    }

    public async Task<bool> CanCreateGuideAsync(Guid userId)
    {
        var remaining = await GetRemainingGuidesTodayAsync(userId);
        return remaining > 0;
    }

    public async Task<bool> CanCreateReviewAsync(Guid userId)
    {
        var remaining = await GetRemainingReviewsTodayAsync(userId);
        return remaining > 0;
    }

    public async Task<bool> CanCreateArticleAsync(Guid userId)
    {
        var remaining = await GetRemainingArticlesTodayAsync(userId);
        return remaining > 0;
    }

    public async Task<int> GetRemainingGuidesTodayAsync(Guid userId)
    {
        var roles = await _roleService.GetUserRolesAsync(userId);
        var countToday = await _guidesRepository.GetCountByUserAndDateAsync(userId, DateTime.UtcNow);

        int limit = 0;
        if (roles.Contains(Roles.Author)) limit = Progression.DailyLimits.Author.Guides;
        else if (roles.Contains(Roles.Reviewer)) limit = Progression.DailyLimits.Reviewer.Guides;
        else if (roles.Contains(Roles.Expert)) limit = Progression.DailyLimits.Expert.Guides;
        else if (roles.Contains(Roles.SuperAdmin) || roles.Contains(Roles.ContentAdmin)) limit = int.MaxValue;

        return Math.Max(0, limit - countToday);
    }

    public async Task<int> GetRemainingReviewsTodayAsync(Guid userId)
    {
        var roles = await _roleService.GetUserRolesAsync(userId);
        var countToday = await _reviewsRepository.GetCountByUserAndDateAsync(userId, DateTime.UtcNow);

        int limit = 0;
        if (roles.Contains(Roles.Author)) limit = Progression.DailyLimits.Author.Reviews;
        else if (roles.Contains(Roles.Reviewer)) limit = Progression.DailyLimits.Reviewer.Reviews;
        else if (roles.Contains(Roles.SuperAdmin) || roles.Contains(Roles.ContentAdmin)) limit = int.MaxValue;

        return Math.Max(0, limit - countToday);
    }

    public async Task<int> GetRemainingArticlesTodayAsync(Guid userId)
    {
        var roles = await _roleService.GetUserRolesAsync(userId);
        var countToday = await _newsRepository.GetCountByUserAndDateAsync(userId, DateTime.UtcNow);

        int limit = 0;
        if (roles.Contains(Roles.Author)) limit = Progression.DailyLimits.Author.Articles;
        else if (roles.Contains(Roles.SuperAdmin) || roles.Contains(Roles.ContentAdmin)) limit = int.MaxValue;

        return Math.Max(0, limit - countToday);
    }

    public async Task<bool> RequestAdminStatusAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || user.TotalPoints < Progression.Thresholds.Master)
            return false;

        user.HasPendingAdminRequest = true;
        await _userRepository.UpdateAsync(user);
        
        _logger.LogInformation("User {UserId} requested Admin status.", userId);
        return true;
    }
}
