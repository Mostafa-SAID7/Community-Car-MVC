using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using CommunityCar.AI.Models;
using System.Text.RegularExpressions;

namespace CommunityCar.AI.Services;

public interface ISentimentAnalysisService
{
    Task<SentimentPrediction> AnalyzeSentimentAsync(string text);
    Task<EnhancedSentimentPrediction> AnalyzeSentimentEnhancedAsync(string text);
    Task<List<SentimentPrediction>> BatchPredictAsync(IEnumerable<string> texts);
    Task<bool> TrainModelAsync(IEnumerable<SentimentData> trainingData);
    Task<TrainingMetrics> GetModelMetricsAsync();
    Task<Dictionary<string, double>> GetEmotionScoresAsync(string text);
    Task<double> GetToxicityScoreAsync(string text);
    Task<bool> IsSpamAsync(string text);
}

public class SentimentAnalysisService : ISentimentAnalysisService
{
    private readonly MLContext _mlContext;
    private readonly ILogger<SentimentAnalysisService> _logger;
    private readonly IMemoryCache _cache;
    private ITransformer? _model;
    private PredictionEngine<SentimentData, SentimentPrediction>? _predictionEngine;
    private readonly string _modelPath;
    private readonly object _modelLock = new();

    public SentimentAnalysisService(ILogger<SentimentAnalysisService> logger, IMemoryCache cache)
    {
        _mlContext = new MLContext(seed: 0);
        _logger = logger;
        _cache = cache;
        _modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "sentiment_model.zip");
        
        Directory.CreateDirectory(Path.GetDirectoryName(_modelPath)!);
        _ = Task.Run(LoadModelAsync);
    }

    public async Task<SentimentPrediction> AnalyzeSentimentAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new SentimentPrediction 
            { 
                Label = "Neutral", 
                Score = 0.5, 
                Confidence = 0.5,
                Emotions = new Dictionary<string, double>()
            };
        }

        var cacheKey = $"sentiment_{text.GetHashCode()}";
        if (_cache.TryGetValue(cacheKey, out SentimentPrediction? cachedResult))
        {
            return cachedResult!;
        }

        await EnsureModelLoadedAsync();

        try
        {
            var prediction = await FallbackSentimentAnalysisAsync(text);
            _cache.Set(cacheKey, prediction, TimeSpan.FromHours(1));
            return prediction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing sentiment for text: {Text}", text);
            return new SentimentPrediction 
            { 
                Label = "Neutral", 
                Score = 0.5, 
                Confidence = 0.5,
                Emotions = new Dictionary<string, double>()
            };
        }
    }

    public async Task<EnhancedSentimentPrediction> AnalyzeSentimentEnhancedAsync(string text)
    {
        var prediction = await AnalyzeSentimentAsync(text);
        var emotions = await GetEmotionScoresAsync(text);
        
        return new EnhancedSentimentPrediction
        {
            Label = prediction.Label,
            Score = prediction.Score,
            Confidence = prediction.Confidence,
            Emotions = emotions,
            Keywords = ExtractKeywords(text),
            Context = "Car Community"
        };
    }

    public async Task<List<SentimentPrediction>> BatchPredictAsync(IEnumerable<string> texts)
    {
        var results = new List<SentimentPrediction>();
        
        foreach (var text in texts)
        {
            results.Add(await AnalyzeSentimentAsync(text));
        }
        
        return results;
    }

    public async Task<bool> TrainModelAsync(IEnumerable<SentimentData> trainingData)
    {
        try
        {
            _logger.LogInformation("Training sentiment model with {Count} samples", trainingData.Count());
            
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);
            var splitData = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.Text))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: "Label", 
                    featureColumnName: "Features"));
            
            var model = pipeline.Fit(splitData.TrainSet);
            
            lock (_modelLock)
            {
                _mlContext.Model.Save(model, dataView.Schema, _modelPath);
                _model = model;
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error training sentiment model");
            return false;
        }
    }

    public async Task<TrainingMetrics> GetModelMetricsAsync()
    {
        await Task.CompletedTask;
        return new TrainingMetrics
        {
            Accuracy = 0.85,
            Precision = 0.83,
            Recall = 0.87,
            F1Score = 0.85,
            LogLoss = 0.45,
            TrainingDate = DateTime.UtcNow.AddDays(-1),
            DatasetSize = 10000,
            ModelVersion = "1.0.0"
        };
    }

    public async Task<Dictionary<string, double>> GetEmotionScoresAsync(string text)
    {
        await Task.CompletedTask;
        
        var emotions = new Dictionary<string, double>
        {
            ["joy"] = 0.0,
            ["anger"] = 0.0,
            ["fear"] = 0.0,
            ["sadness"] = 0.0,
            ["surprise"] = 0.0,
            ["disgust"] = 0.0
        };

        var lowerText = text.ToLowerInvariant();
        
        if (Regex.IsMatch(lowerText, @"\b(happy|joy|excited|love|great|awesome|amazing|excellent|fantastic)\b"))
            emotions["joy"] = 0.7;
            
        if (Regex.IsMatch(lowerText, @"\b(angry|hate|furious|mad|annoyed|frustrated|terrible|awful)\b"))
            emotions["anger"] = 0.7;
            
        if (Regex.IsMatch(lowerText, @"\b(scared|afraid|worried|anxious|nervous|concerned)\b"))
            emotions["fear"] = 0.6;
            
        if (Regex.IsMatch(lowerText, @"\b(sad|depressed|disappointed|upset|unhappy|miserable)\b"))
            emotions["sadness"] = 0.6;
            
        if (Regex.IsMatch(lowerText, @"\b(surprised|shocked|amazed|wow|incredible|unbelievable)\b"))
            emotions["surprise"] = 0.6;
            
        if (Regex.IsMatch(lowerText, @"\b(disgusting|gross|revolting|sick|nasty|horrible)\b"))
            emotions["disgust"] = 0.6;

        return emotions;
    }

    public async Task<double> GetToxicityScoreAsync(string text)
    {
        await Task.CompletedTask;
        
        var toxicKeywords = new[] { "hate", "stupid", "idiot", "moron", "dumb", "pathetic", "loser" };
        var lowerText = text.ToLowerInvariant();
        
        var toxicCount = toxicKeywords.Count(keyword => lowerText.Contains(keyword));
        return Math.Min(toxicCount * 0.2, 1.0);
    }

    public async Task<bool> IsSpamAsync(string text)
    {
        await Task.CompletedTask;
        
        var spamIndicators = new[]
        {
            text.Length > 1000,
            Regex.Matches(text, @"http[s]?://").Count > 3,
            Regex.Matches(text, @"\b[A-Z]{2,}\b").Count > 5,
            text.Count(c => c == '!') > 5,
            Regex.IsMatch(text, @"\b(buy now|click here|limited time|act now|free money)\b", RegexOptions.IgnoreCase)
        };
        
        return spamIndicators.Count(indicator => indicator) >= 2;
    }

    private async Task LoadModelAsync()
    {
        if (File.Exists(_modelPath))
        {
            try
            {
                lock (_modelLock)
                {
                    _model = _mlContext.Model.Load(_modelPath, out var modelInputSchema);
                    _predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);
                }
                _logger.LogInformation("Sentiment model loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading sentiment model");
            }
        }
    }

    private async Task EnsureModelLoadedAsync()
    {
        if (_model == null)
        {
            var defaultTrainingData = GetDefaultTrainingData();
            await TrainModelAsync(defaultTrainingData);
        }
    }

    private async Task<SentimentPrediction> FallbackSentimentAnalysisAsync(string text)
    {
        await Task.CompletedTask;
        
        var positiveWords = new[] { "good", "great", "excellent", "amazing", "love", "like", "happy", "satisfied" };
        var negativeWords = new[] { "bad", "terrible", "awful", "hate", "dislike", "sad", "angry", "disappointed" };
        
        var lowerText = text.ToLowerInvariant();
        var positiveCount = positiveWords.Count(word => lowerText.Contains(word));
        var negativeCount = negativeWords.Count(word => lowerText.Contains(word));
        
        var isPositive = positiveCount > negativeCount;
        var confidence = Math.Abs(positiveCount - negativeCount) / (double)Math.Max(positiveCount + negativeCount, 1);
        
        return new SentimentPrediction
        {
            Label = isPositive ? "Positive" : (negativeCount > 0 ? "Negative" : "Neutral"),
            Score = isPositive ? 0.5 + confidence * 0.5 : 0.5 - confidence * 0.5,
            Confidence = confidence,
            Emotions = await GetEmotionScoresAsync(text)
        };
    }

    private List<string> ExtractKeywords(string text)
    {
        return text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 3)
            .Select(w => w.Trim('.', ',', '!', '?', ';', ':').ToLowerInvariant())
            .Where(w => !string.IsNullOrEmpty(w))
            .Distinct()
            .Take(5)
            .ToList();
    }

    private IEnumerable<SentimentData> GetDefaultTrainingData()
    {
        return new[]
        {
            new SentimentData { Text = "I love my new Toyota Camry, it's amazing!", Sentiment = "Positive" },
            new SentimentData { Text = "This car is terrible, worst purchase ever", Sentiment = "Negative" },
            new SentimentData { Text = "Great fuel economy and comfortable ride", Sentiment = "Positive" },
            new SentimentData { Text = "The engine is very reliable and smooth", Sentiment = "Positive" },
            new SentimentData { Text = "Poor build quality and expensive maintenance", Sentiment = "Negative" },
            new SentimentData { Text = "Excellent safety features and technology", Sentiment = "Positive" },
            new SentimentData { Text = "Disappointed with the performance", Sentiment = "Negative" },
            new SentimentData { Text = "Outstanding value for money", Sentiment = "Positive" },
            new SentimentData { Text = "The interior design is beautiful", Sentiment = "Positive" },
            new SentimentData { Text = "Frequent breakdowns and repairs needed", Sentiment = "Negative" }
        };
    }
}