namespace CommunityCar.Application.Common.Models.Caching;

/// <summary>
/// Cache expiration settings for different types of data
/// </summary>
public static class CacheSettings
{
    // Short-term cache (1-5 minutes) - Frequently changing data
    public static readonly TimeSpan VeryShort = TimeSpan.FromMinutes(1);
    public static readonly TimeSpan Short = TimeSpan.FromMinutes(5);

    // Medium-term cache (10-30 minutes) - Moderately changing data
    public static readonly TimeSpan Medium = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan MediumLong = TimeSpan.FromMinutes(30);

    // Long-term cache (1-6 hours) - Slowly changing data
    public static readonly TimeSpan Long = TimeSpan.FromHours(1);
    public static readonly TimeSpan VeryLong = TimeSpan.FromHours(6);

    // Extended cache (12-24 hours) - Rarely changing data
    public static readonly TimeSpan Extended = TimeSpan.FromHours(12);
    public static readonly TimeSpan Daily = TimeSpan.FromDays(1);

    // Specific cache durations by data type
    public static class User
    {
        public static readonly TimeSpan Profile = MediumLong; // 30 minutes
        public static readonly TimeSpan PublicProfile = Long; // 1 hour
        public static readonly TimeSpan Settings = Long; // 1 hour
        public static readonly TimeSpan Interests = MediumLong; // 30 minutes
        public static readonly TimeSpan Friends = Medium; // 15 minutes
        public static readonly TimeSpan Gallery = MediumLong; // 30 minutes
        public static readonly TimeSpan Activities = Short; // 5 minutes
        public static readonly TimeSpan Notifications = VeryShort; // 1 minute
    }

    public static class Profile
    {
        public static readonly TimeSpan Statistics = Medium; // 15 minutes
        public static readonly TimeSpan Achievements = MediumLong; // 30 minutes
        public static readonly TimeSpan Badges = MediumLong; // 30 minutes
        public static readonly TimeSpan Points = Medium; // 15 minutes
        public static readonly TimeSpan Level = MediumLong; // 30 minutes
        public static readonly TimeSpan SearchResults = Medium; // 15 minutes
    }

    public static class Feed
    {
        public static readonly TimeSpan PersonalizedFeed = Short; // 5 minutes
        public static readonly TimeSpan TrendingTopics = Medium; // 15 minutes
        public static readonly TimeSpan ActiveStories = VeryShort; // 1 minute
        public static readonly TimeSpan SuggestedFriends = MediumLong; // 30 minutes
        public static readonly TimeSpan FeedStats = Short; // 5 minutes
        public static readonly TimeSpan PopularContent = Medium; // 15 minutes
    }

    public static class Community
    {
        public static readonly TimeSpan Groups = MediumLong; // 30 minutes
        public static readonly TimeSpan Events = Medium; // 15 minutes
        public static readonly TimeSpan News = Short; // 5 minutes
        public static readonly TimeSpan Reviews = MediumLong; // 30 minutes
        public static readonly TimeSpan Posts = Short; // 5 minutes
        public static readonly TimeSpan Stories = VeryShort; // 1 minute
        public static readonly TimeSpan QA = Medium; // 15 minutes
        public static readonly TimeSpan Guides = Long; // 1 hour
    }

    public static class Gamification
    {
        public static readonly TimeSpan UserBadges = MediumLong; // 30 minutes
        public static readonly TimeSpan AvailableBadges = Daily; // 24 hours
        public static readonly TimeSpan UserPoints = Medium; // 15 minutes
        public static readonly TimeSpan UserLevel = MediumLong; // 30 minutes
        public static readonly TimeSpan Leaderboard = Medium; // 15 minutes
        public static readonly TimeSpan AchievementProgress = Medium; // 15 minutes
    }

    public static class Content
    {
        public static readonly TimeSpan Comments = Short; // 5 minutes
        public static readonly TimeSpan Reactions = VeryShort; // 1 minute
        public static readonly TimeSpan Views = Short; // 5 minutes
        public static readonly TimeSpan Shares = Short; // 5 minutes
        public static readonly TimeSpan Bookmarks = Medium; // 15 minutes
    }

    public static class Reference
    {
        public static readonly TimeSpan Categories = Daily; // 24 hours
        public static readonly TimeSpan Tags = VeryLong; // 6 hours
        public static readonly TimeSpan CarMakes = Daily; // 24 hours
        public static readonly TimeSpan CarModels = Daily; // 24 hours
        public static readonly TimeSpan Locations = Daily; // 24 hours
        public static readonly TimeSpan LocalizationStrings = Daily; // 24 hours
    }
}


