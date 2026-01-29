namespace CommunityCar.Application.Common.Models.Caching;

/// <summary>
/// Centralized cache key management for consistent caching across the application
/// </summary>
public static class CacheKeys
{
    // Cache key prefixes
    private const string USER_PREFIX = "user";
    private const string PROFILE_PREFIX = "profile";
    private const string FEED_PREFIX = "feed";
    private const string COMMUNITY_PREFIX = "community";
    private const string GAMIFICATION_PREFIX = "gamification";
    private const string CONTENT_PREFIX = "content";
    private const string REFERENCE_PREFIX = "reference";

    // User-related cache keys
    public static class User
    {
        public static string Profile(Guid userId) => $"{USER_PREFIX}:profile:{userId}";
        public static string PublicProfile(Guid userId) => $"{USER_PREFIX}:public-profile:{userId}";
        public static string Settings(Guid userId) => $"{USER_PREFIX}:settings:{userId}";
        public static string Interests(Guid userId) => $"{USER_PREFIX}:interests:{userId}";
        public static string Friends(Guid userId) => $"{USER_PREFIX}:friends:{userId}";
        public static string FriendIds(Guid userId) => $"{USER_PREFIX}:friend-ids:{userId}";
        public static string Gallery(Guid userId) => $"{USER_PREFIX}:gallery:{userId}";
        public static string Activities(Guid userId) => $"{USER_PREFIX}:activities:{userId}";
        public static string Notifications(Guid userId) => $"{USER_PREFIX}:notifications:{userId}";
    }

    // Profile-related cache keys
    public static class Profile
    {
        public static string Statistics(Guid userId) => $"{PROFILE_PREFIX}:stats:{userId}";
        public static string Achievements(Guid userId) => $"{PROFILE_PREFIX}:achievements:{userId}";
        public static string Badges(Guid userId) => $"{PROFILE_PREFIX}:badges:{userId}";
        public static string Points(Guid userId) => $"{PROFILE_PREFIX}:points:{userId}";
        public static string Level(Guid userId) => $"{PROFILE_PREFIX}:level:{userId}";
        public static string SearchResults(string searchTerm, int page) => $"{PROFILE_PREFIX}:search:{searchTerm}:{page}";
    }

    // Feed-related cache keys
    public static class Feed
    {
        public static string PersonalizedFeed(Guid userId, int page) => $"{FEED_PREFIX}:personalized:{userId}:{page}";
        public static string TrendingTopics(int count) => $"{FEED_PREFIX}:trending-topics:{count}";
        public static string ActiveStories(Guid userId) => $"{FEED_PREFIX}:active-stories:{userId}";
        public static string SuggestedFriends(Guid userId, int count) => $"{FEED_PREFIX}:suggested-friends:{userId}:{count}";
        public static string FeedStats(Guid userId) => $"{FEED_PREFIX}:stats:{userId}";
        public static string PopularContent(int hours) => $"{FEED_PREFIX}:popular-content:{hours}";
    }

    // Community-related cache keys
    public static class Community
    {
        public static string Groups(Guid userId) => $"{COMMUNITY_PREFIX}:groups:{userId}";
        public static string Events(string location, DateTime date) => $"{COMMUNITY_PREFIX}:events:{location}:{date:yyyyMMdd}";
        public static string News(string category, int page) => $"{COMMUNITY_PREFIX}:news:{category}:{page}";
        public static string Reviews(Guid itemId) => $"{COMMUNITY_PREFIX}:reviews:{itemId}";
        public static string Posts(string category, int page) => $"{COMMUNITY_PREFIX}:posts:{category}:{page}";
        public static string Stories(Guid userId) => $"{COMMUNITY_PREFIX}:stories:{userId}";
        public static string QA(string category, int page) => $"{COMMUNITY_PREFIX}:qa:{category}:{page}";
        public static string Guides(string category) => $"{COMMUNITY_PREFIX}:guides:{category}";
    }

    // Gamification cache keys
    public static class Gamification
    {
        public static string UserBadges(Guid userId) => $"{GAMIFICATION_PREFIX}:user-badges:{userId}";
        public static string AvailableBadges() => $"{GAMIFICATION_PREFIX}:available-badges";
        public static string UserPoints(Guid userId) => $"{GAMIFICATION_PREFIX}:user-points:{userId}";
        public static string UserLevel(Guid userId) => $"{GAMIFICATION_PREFIX}:user-level:{userId}";
        public static string Leaderboard(string type, int count) => $"{GAMIFICATION_PREFIX}:leaderboard:{type}:{count}";
        public static string AchievementProgress(Guid userId) => $"{GAMIFICATION_PREFIX}:achievement-progress:{userId}";
    }

    // Content cache keys
    public static class Content
    {
        public static string Comments(Guid contentId) => $"{CONTENT_PREFIX}:comments:{contentId}";
        public static string Reactions(Guid contentId) => $"{CONTENT_PREFIX}:reactions:{contentId}";
        public static string Views(Guid contentId) => $"{CONTENT_PREFIX}:views:{contentId}";
        public static string Shares(Guid contentId) => $"{CONTENT_PREFIX}:shares:{contentId}";
        public static string Bookmarks(Guid userId) => $"{CONTENT_PREFIX}:bookmarks:{userId}";
    }

    // Reference data cache keys
    public static class Reference
    {
        public static string Categories() => $"{REFERENCE_PREFIX}:categories";
        public static string Tags() => $"{REFERENCE_PREFIX}:tags";
        public static string CarMakes() => $"{REFERENCE_PREFIX}:car-makes";
        public static string CarModels(string make) => $"{REFERENCE_PREFIX}:car-models:{make}";
        public static string Locations() => $"{REFERENCE_PREFIX}:locations";
        public static string LocalizationStrings(string culture) => $"{REFERENCE_PREFIX}:localization:{culture}";
    }

    // Cache invalidation patterns
    public static class Patterns
    {
        public static string UserData(Guid userId) => $"{USER_PREFIX}:{userId}:*";
        public static string ProfileData(Guid userId) => $"{PROFILE_PREFIX}:{userId}:*";
        public static string FeedData(Guid userId) => $"{FEED_PREFIX}:{userId}:*";
        public static string AllFeedData() => $"{FEED_PREFIX}:*";
        public static string GamificationData(Guid userId) => $"{GAMIFICATION_PREFIX}:{userId}:*";
        public static string ContentData(Guid contentId) => $"{CONTENT_PREFIX}:{contentId}:*";
        public static string ReferenceData() => $"{REFERENCE_PREFIX}:*";
    }
}


