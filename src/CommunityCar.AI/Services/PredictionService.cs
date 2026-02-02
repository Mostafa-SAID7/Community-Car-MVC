using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.Extensions.Logging;
using CommunityCar.AI.Models;
using CommunityCar.AI.Configuration;
using CommunityCar.Domain.Enums.AI;
using Microsoft.Extensions.Options;

namespace CommunityCar.AI.Services;

/// <summary>
/// Advanced prediction service for user behavior and content recommendations
/// </summary>
public interface IPredictionService
{
    Task<UserBehaviorPrediction> PredictUserBehaviorAsync(UserBehaviorData userData);
    Task<List<string>> GetContentRecommendationsAsync(string userId, int count = 10);
    Task<float> PredictEngagementAsync(string content, string userId);
    Task<List<string>> PredictTrendingTopicsAsync(int count = 5);
    Task<Dictionary<string, float>> GetUserInterestScoresAsync(string userId);
    Task TrainUserBehaviorModelAsync(List<UserBehaviorData> trainingData);
    Task<bool> IsUserAtRiskAsync(string userId);
    Task<List<string>> GetPersonalizedNotificationsAsync(string userId);
}

public class PredictionService : IPredictionService
{
    private readonly MLContext _mlContext;
    private readonly ILogger<PredictionService> _logger;
    private readonly AISettings _settings;
    private ITransformer? _userBehaviorModel;
    private ITransformer? _engagementModel;
    private ITransformer? _recommendationModel;
    private readonly string _userBehaviorModelPath;
    private readonly string _engagementModelPath;
    private readonly string _recommendationModelPath;

    public PredictionService(
        ILogger<PredictionService> logger,
        IOptions<AISettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
        _mlContext = new MLContext(seed: 0);
        
        // Model file paths
        _userBehaviorModelPath = Path.Combine("Models", "user_behavior_model.zip");
        _engagementModelPath = Path.Combine("Models", "engagement_model.zip");
        _recommendationModelPath = Path.Combine("Models", "recommendation_model.zip");
        
        // Ensure Models directory exists
        Directory.CreateDirectory("Models");
        
        // Load existing models if available
        _ = Task.Run(LoadModelsAsync);
    }

    public async Task<UserBehaviorPrediction> PredictUserBehaviorAsync(UserBehaviorData userData)
    {
        try
        {
            var prediction = new UserBehaviorPrediction
            {
                UserId = userData.UserId
            };

            // Calculate churn probability
            prediction.ChurnProbability = await CalculateChurnProbabilityAsync(userData);
            
            // Calculate engagement score
            prediction.EngagementScore = await CalculateEngagementScoreAsync(userData);
            
            // Calculate toxicity risk
            prediction.ToxicityRisk = await CalculateToxicityRiskAsync(userData);
            
            // Determine user segment
            prediction.Segment = await DetermineUserSegmentAsync(userData, prediction);
            
            // Generate recommended actions
            prediction.RecommendedActions = await GenerateRecommendedActionsAsync(userData, prediction);

            _logger.LogInformation("Predicted user behavior for {UserId}: Segment={Segment}, ChurnProb={ChurnProb}, Engagement={Engagement}",
                userData.UserId, prediction.Segment, prediction.ChurnProbability, prediction.EngagementScore);

            return prediction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error predicting user behavior for {UserId}", userData.UserId);
            return new UserBehaviorPrediction
            {
                UserId = userData.UserId,
                Segment = UserSegmentType.NewUser
            };
        }
    }

    public async Task<List<string>> GetContentRecommendationsAsync(string userId, int count = 10)
    {
        try
        {
            // This would use a trained recommendation model
            // For now, return rule-based recommendations
            var recommendations = new List<string>();

            // Get user interests and behavior patterns
            var userInterests = await GetUserInterestScoresAsync(userId);
            var topInterests = userInterests.OrderByDescending(x => x.Value).Take(3).Select(x => x.Key).ToList();

            // Generate content recommendations based on interests
            foreach (var interest in topInterests)
            {
                recommendations.AddRange(await GetContentByInterestAsync(interest, count / 3));
            }

            // Add trending content
            var trendingTopics = await PredictTrendingTopicsAsync(count / 4);
            recommendations.AddRange(trendingTopics);

            return recommendations.Distinct().Take(count).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content recommendations for {UserId}", userId);
            return new List<string>();
        }
    }

    public async Task<float> PredictEngagementAsync(string content, string userId)
    {
        try
        {
            // Rule-based engagement prediction
            var engagementScore = 0.5f; // Base score

            // Content length factor
            var contentLength = content.Length;
            if (contentLength > 50 && contentLength < 500)
                engagementScore += 0.1f;

            // Question factor
            if (content.Contains("?"))
                engagementScore += 0.15f;

            // Hashtag factor
            var hashtagCount = content.Count(c => c == '#');
            engagementScore += Math.Min(0.1f, hashtagCount * 0.05f);

            // Mention factor
            var mentionCount = content.Count(c => c == '@');
            engagementScore += Math.Min(0.1f, mentionCount * 0.03f);

            // Time-based factor (posts during peak hours get higher scores)
            var currentHour = DateTime.Now.Hour;
            if ((currentHour >= 9 && currentHour <= 11) || (currentHour >= 19 && currentHour <= 21))
                engagementScore += 0.1f;

            return await Task.FromResult(Math.Min(1.0f, engagementScore));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error predicting engagement");
            return 0.5f;
        }
    }

    public async Task<List<string>> PredictTrendingTopicsAsync(int count = 5)
    {
        try
        {
            // This would analyze recent content and engagement patterns
            // For now, return some automotive-related trending topics
            var trendingTopics = new List<string>
            {
                "Electric Vehicles",
                "Car Maintenance Tips",
                "Fuel Efficiency",
                "Auto Insurance",
                "Road Safety",
                "Car Reviews",
                "Driving Tips",
                "Auto Technology",
                "Car Modifications",
                "Vehicle Troubleshooting"
            };

            // Shuffle and return requested count
            var random = new Random();
            return await Task.FromResult(trendingTopics.OrderBy(x => random.Next()).Take(count).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error predicting trending topics");
            return new List<string>();
        }
    }

    public async Task<Dictionary<string, float>> GetUserInterestScoresAsync(string userId)
    {
        try
        {
            // This would analyze user's past interactions, posts, and engagement
            // For now, return sample interest scores
            var interests = new Dictionary<string, float>
            {
                { "Automotive", 0.8f },
                { "Technology", 0.6f },
                { "Community", 0.7f },
                { "Maintenance", 0.5f },
                { "Reviews", 0.4f },
                { "Safety", 0.6f },
                { "Performance", 0.3f },
                { "Electric Vehicles", 0.7f },
                { "Modifications", 0.2f },
                { "Insurance", 0.4f }
            };

            return await Task.FromResult(interests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user interest scores for {UserId}", userId);
            return new Dictionary<string, float>();
        }
    }

    public async Task TrainUserBehaviorModelAsync(List<UserBehaviorData> trainingData)
    {
        try
        {
            _logger.LogInformation("Starting user behavior model training with {Count} samples", trainingData.Count);

            // Create data view
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            // Define training pipeline for binary classification (active/inactive)
            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(UserBehaviorData.PostsCount),
                    nameof(UserBehaviorData.CommentsCount),
                    nameof(UserBehaviorData.LikesGiven),
                    nameof(UserBehaviorData.LikesReceived),
                    nameof(UserBehaviorData.SharesCount),
                    nameof(UserBehaviorData.EngagementRate),
                    nameof(UserBehaviorData.DaysActive),
                    nameof(UserBehaviorData.AverageSentiment),
                    nameof(UserBehaviorData.ReportsReceived))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(UserBehaviorData.IsActive),
                    featureColumnName: "Features"));

            // Train the model
            _userBehaviorModel = pipeline.Fit(dataView);

            // Save the model
            _mlContext.Model.Save(_userBehaviorModel, dataView.Schema, _userBehaviorModelPath);

            _logger.LogInformation("User behavior model training completed and saved to {ModelPath}", _userBehaviorModelPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error training user behavior model");
            throw;
        }
    }

    public async Task<bool> IsUserAtRiskAsync(string userId)
    {
        try
        {
            // This would use the trained model to predict if user is at risk of churning
            // For now, use rule-based logic
            
            // Get user behavior data (this would come from database)
            var userData = await GetUserBehaviorDataAsync(userId);
            var prediction = await PredictUserBehaviorAsync(userData);

            return prediction.ChurnProbability > 0.7f || 
                   prediction.ToxicityRisk > 0.8f || 
                   prediction.EngagementScore < 0.3f;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user is at risk: {UserId}", userId);
            return false;
        }
    }

    public async Task<List<string>> GetPersonalizedNotificationsAsync(string userId)
    {
        try
        {
            var notifications = new List<string>();
            var userInterests = await GetUserInterestScoresAsync(userId);
            var userData = await GetUserBehaviorDataAsync(userId);

            // Generate personalized notifications based on user behavior and interests
            if (userData.EngagementRate < 0.3f)
            {
                notifications.Add("Check out the latest discussions in your favorite topics!");
            }

            if (userInterests.ContainsKey("Automotive") && userInterests["Automotive"] > 0.7f)
            {
                notifications.Add("New automotive tips and tricks are available!");
            }

            if (userData.DaysActive > 30 && userData.PostsCount < 5)
            {
                notifications.Add("Share your car experiences with the community!");
            }

            var trendingTopics = await PredictTrendingTopicsAsync(3);
            foreach (var topic in trendingTopics.Take(2))
            {
                notifications.Add($"Trending now: {topic} - Join the conversation!");
            }

            return notifications.Take(5).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting personalized notifications for {UserId}", userId);
            return new List<string>();
        }
    }

    private async Task LoadModelsAsync()
    {
        try
        {
            if (File.Exists(_userBehaviorModelPath))
            {
                _userBehaviorModel = _mlContext.Model.Load(_userBehaviorModelPath, out _);
                _logger.LogInformation("Loaded user behavior model from {ModelPath}", _userBehaviorModelPath);
            }

            if (File.Exists(_engagementModelPath))
            {
                _engagementModel = _mlContext.Model.Load(_engagementModelPath, out _);
                _logger.LogInformation("Loaded engagement model from {ModelPath}", _engagementModelPath);
            }

            if (File.Exists(_recommendationModelPath))
            {
                _recommendationModel = _mlContext.Model.Load(_recommendationModelPath, out _);
                _logger.LogInformation("Loaded recommendation model from {ModelPath}", _recommendationModelPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading prediction models");
        }
    }

    private async Task<float> CalculateChurnProbabilityAsync(UserBehaviorData userData)
    {
        var churnScore = 0.0f;

        // Low activity indicators
        if (userData.PostsCount < 2) churnScore += 0.2f;
        if (userData.CommentsCount < 5) churnScore += 0.15f;
        if (userData.EngagementRate < 0.1f) churnScore += 0.25f;
        if (userData.DaysActive < 7) churnScore += 0.1f;

        // Negative sentiment indicator
        if (userData.AverageSentiment < 0.3f) churnScore += 0.15f;

        // Reports received indicator
        if (userData.ReportsReceived > 2) churnScore += 0.15f;

        return await Task.FromResult(Math.Min(1.0f, churnScore));
    }

    private async Task<float> CalculateEngagementScoreAsync(UserBehaviorData userData)
    {
        var engagementScore = 0.0f;

        // Activity-based scoring
        engagementScore += Math.Min(0.3f, userData.PostsCount * 0.05f);
        engagementScore += Math.Min(0.2f, userData.CommentsCount * 0.02f);
        engagementScore += Math.Min(0.2f, userData.LikesGiven * 0.01f);
        engagementScore += Math.Min(0.15f, userData.SharesCount * 0.03f);
        engagementScore += Math.Min(0.15f, userData.EngagementRate);

        return await Task.FromResult(Math.Min(1.0f, engagementScore));
    }

    private async Task<float> CalculateToxicityRiskAsync(UserBehaviorData userData)
    {
        var toxicityRisk = 0.0f;

        // Reports received
        toxicityRisk += Math.Min(0.5f, userData.ReportsReceived * 0.2f);

        // Low sentiment
        if (userData.AverageSentiment < 0.3f) toxicityRisk += 0.3f;

        // Low engagement with high activity (potential spam)
        if (userData.PostsCount > 10 && userData.EngagementRate < 0.1f) toxicityRisk += 0.2f;

        return await Task.FromResult(Math.Min(1.0f, toxicityRisk));
    }

    private async Task<UserSegmentType> DetermineUserSegmentAsync(UserBehaviorData userData, UserBehaviorPrediction prediction)
    {
        if (prediction.ToxicityRisk > 0.8f) return UserSegmentType.ToxicUser;
        if (prediction.ChurnProbability > 0.8f) return UserSegmentType.ChurnedUser;
        if (prediction.ChurnProbability > 0.6f) return UserSegmentType.AtRiskUser;
        if (userData.DaysActive < 7) return UserSegmentType.NewUser;
        if (prediction.EngagementScore > 0.8f) return UserSegmentType.PowerUser;
        
        return await Task.FromResult(UserSegmentType.ActiveUser);
    }

    private async Task<List<string>> GenerateRecommendedActionsAsync(UserBehaviorData userData, UserBehaviorPrediction prediction)
    {
        var actions = new List<string>();

        switch (prediction.Segment)
        {
            case UserSegmentType.NewUser:
                actions.Add("Complete profile setup");
                actions.Add("Join relevant groups");
                actions.Add("Follow community guidelines tutorial");
                break;

            case UserSegmentType.AtRiskUser:
                actions.Add("Send personalized content recommendations");
                actions.Add("Invite to participate in discussions");
                actions.Add("Offer community challenges");
                break;

            case UserSegmentType.PowerUser:
                actions.Add("Invite to become community moderator");
                actions.Add("Feature their content");
                actions.Add("Provide early access to new features");
                break;

            case UserSegmentType.ToxicUser:
                actions.Add("Review recent activity");
                actions.Add("Send community guidelines reminder");
                actions.Add("Consider temporary restrictions");
                break;
        }

        return await Task.FromResult(actions);
    }

    private async Task<List<string>> GetContentByInterestAsync(string interest, int count)
    {
        // This would query the database for content related to the interest
        // For now, return sample content IDs
        var contentIds = new List<string>();
        for (int i = 0; i < count; i++)
        {
            contentIds.Add($"{interest.ToLower()}_content_{i + 1}");
        }
        return await Task.FromResult(contentIds);
    }

    private async Task<UserBehaviorData> GetUserBehaviorDataAsync(string userId)
    {
        // This would fetch real data from the database
        // For now, return sample data
        return await Task.FromResult(new UserBehaviorData
        {
            UserId = userId,
            PostsCount = 5,
            CommentsCount = 15,
            LikesGiven = 25,
            LikesReceived = 20,
            SharesCount = 3,
            EngagementRate = 0.6f,
            DaysActive = 30,
            AverageSentiment = 0.7f,
            ReportsReceived = 0,
            IsActive = true,
            UserSegment = UserSegmentType.ActiveUser.ToString()
        });
    }
}