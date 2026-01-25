using CommunityCar.Application.Common.Interfaces.Services.Profile;
using CommunityCar.Application.Features.Profile.DTOs;
using CommunityCar.Domain.Entities.Profile;

namespace CommunityCar.Application.Services.Profile;

public class GamificationService : IGamificationService
{
    public async Task<UserGamificationStatsDTO> GetUserStatsAsync(Guid userId)
    {
        // Mock implementation - in real app, calculate from database
        await Task.Delay(1);
        
        return new UserGamificationStatsDTO
        {
            TotalPoints = 1250,
            Level = 5,
            PointsToNextLevel = 250,
            TotalBadges = 8,
            CompletedAchievements = 12,
            TotalAchievements = 25,
            RecentBadges = await GetRecentBadges(userId),
            InProgressAchievements = await GetInProgressAchievements(userId)
        };
    }

    public async Task<List<UserBadgeDTO>> GetUserBadgesAsync(Guid userId, bool displayedOnly = false)
    {
        // Mock implementation
        await Task.Delay(1);
        
        var badges = new List<UserBadgeDTO>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BadgeId = "first_post",
                Name = "First Steps",
                Description = "Posted your first content",
                IconUrl = "/images/badges/first-post.svg",
                Category = BadgeCategory.Content,
                Rarity = BadgeRarity.Common,
                Points = 50,
                EarnedAt = DateTime.UtcNow.AddDays(-10),
                IsDisplayed = true,
                NameAr = "الخطوات الأولى",
                DescriptionAr = "قمت بنشر محتواك الأول",
                RarityColor = "text-gray-500",
                CategoryName = "Content Creator"
            },
            new()
            {
                Id = Guid.NewGuid(),
                BadgeId = "community_helper",
                Name = "Community Helper",
                Description = "Helped 10 community members",
                IconUrl = "/images/badges/helper.svg",
                Category = BadgeCategory.Community,
                Rarity = BadgeRarity.Uncommon,
                Points = 150,
                EarnedAt = DateTime.UtcNow.AddDays(-5),
                IsDisplayed = true,
                RarityColor = "text-green-500",
                CategoryName = "Community"
            },
            new()
            {
                Id = Guid.NewGuid(),
                BadgeId = "car_enthusiast",
                Name = "Car Enthusiast",
                Description = "Shared 25 automotive posts",
                IconUrl = "/images/badges/car-enthusiast.svg",
                Category = BadgeCategory.Automotive,
                Rarity = BadgeRarity.Rare,
                Points = 300,
                EarnedAt = DateTime.UtcNow.AddDays(-2),
                IsDisplayed = true,
                RarityColor = "text-blue-500",
                CategoryName = "Automotive"
            }
        };

        return displayedOnly ? badges.Where(b => b.IsDisplayed).ToList() : badges;
    }

    public async Task<List<UserAchievementDTO>> GetUserAchievementsAsync(Guid userId, bool completedOnly = false)
    {
        // Mock implementation
        await Task.Delay(1);
        
        var achievements = new List<UserAchievementDTO>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AchievementId = "social_butterfly",
                Title = "Social Butterfly",
                Description = "Make 50 friends in the community",
                CurrentProgress = 23,
                RequiredProgress = 50,
                IsCompleted = false,
                RewardPoints = 200,
                RewardBadgeId = "social_master",
                TitleAr = "الفراشة الاجتماعية",
                DescriptionAr = "تكوين 50 صديقًا في المجتمع",
                ProgressPercentage = 46
            },
            new()
            {
                Id = Guid.NewGuid(),
                AchievementId = "content_creator",
                Title = "Content Creator",
                Description = "Create 100 posts",
                CurrentProgress = 67,
                RequiredProgress = 100,
                IsCompleted = false,
                RewardPoints = 500,
                RewardBadgeId = "creator_master",
                ProgressPercentage = 67
            },
            new()
            {
                Id = Guid.NewGuid(),
                AchievementId = "helpful_member",
                Title = "Helpful Member",
                Description = "Receive 100 likes on your posts",
                CurrentProgress = 100,
                RequiredProgress = 100,
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow.AddDays(-1),
                RewardPoints = 300,
                RewardBadgeId = "helper_badge",
                ProgressPercentage = 100
            }
        };

        return completedOnly ? achievements.Where(a => a.IsCompleted).ToList() : achievements;
    }

    public async Task<bool> AwardBadgeAsync(Guid userId, string badgeId)
    {
        // Mock implementation - in real app, create UserBadge entity
        await Task.Delay(1);
        return true;
    }

    public async Task<bool> UpdateAchievementProgressAsync(Guid userId, string achievementId, int progress)
    {
        // Mock implementation
        await Task.Delay(1);
        return true;
    }

    public async Task<bool> ToggleBadgeDisplayAsync(Guid badgeId)
    {
        // Mock implementation
        await Task.Delay(1);
        return true;
    }

    public async Task InitializeUserGamificationAsync(Guid userId)
    {
        // Mock implementation - in real app, create initial achievements and award welcome badge
        await Task.Delay(1);
    }

    public async Task ProcessUserActionAsync(Guid userId, string actionType, Dictionary<string, object>? metadata = null)
    {
        // Mock implementation - in real app, process user actions and update achievements
        await Task.Delay(1);
        
        // Example logic:
        // switch (actionType)
        // {
        //     case "post_created":
        //         await UpdateAchievementProgressAsync(userId, "content_creator", 1);
        //         break;
        //     case "friend_added":
        //         await UpdateAchievementProgressAsync(userId, "social_butterfly", 1);
        //         break;
        // }
    }

    private async Task<List<UserBadgeDTO>> GetRecentBadges(Guid userId)
    {
        var allBadges = await GetUserBadgesAsync(userId);
        return allBadges.OrderByDescending(b => b.EarnedAt).Take(3).ToList();
    }

    private async Task<List<UserAchievementDTO>> GetInProgressAchievements(Guid userId)
    {
        var allAchievements = await GetUserAchievementsAsync(userId);
        return allAchievements.Where(a => !a.IsCompleted).OrderByDescending(a => a.ProgressPercentage).Take(3).ToList();
    }
}