using Microsoft.EntityFrameworkCore;
using CommunityCar.Domain.Entities.AI;
using CommunityCar.Infrastructure.Persistence.Data;
using System.Text.Json;

namespace CommunityCar.Infrastructure.Persistence.Seeding.AI;

public static class AIModelSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.AIModels.AnyAsync())
            return;

        var models = new List<AIModel>
        {
            new AIModel
            {
                Name = "Intent Classification",
                Description = "Classifies user intents in chat messages to provide appropriate responses",
                Version = "v2.1",
                Type = AIModelType.IntentClassification,
                Status = AIModelStatus.Active,
                Accuracy = 94.2,
                DatasetSize = 15000,
                LastTrained = DateTime.UtcNow.AddDays(-7),
                LastUsed = DateTime.UtcNow.AddHours(-2),
                Configuration = JsonSerializer.Serialize(new
                {
                    MaxTokens = 512,
                    Temperature = 0.7,
                    TopP = 0.9,
                    Classes = new[] { "question", "complaint", "compliment", "request", "greeting", "goodbye" }
                }),
                ModelPath = "/models/intent_classification_v2.1.pkl",
                IsActive = true
            },
            new AIModel
            {
                Name = "Sentiment Analysis",
                Description = "Analyzes sentiment of user messages to gauge satisfaction and mood",
                Version = "v1.8",
                Type = AIModelType.SentimentAnalysis,
                Status = AIModelStatus.Active,
                Accuracy = 91.7,
                DatasetSize = 12000,
                LastTrained = DateTime.UtcNow.AddDays(-5),
                LastUsed = DateTime.UtcNow.AddMinutes(-15),
                Configuration = JsonSerializer.Serialize(new
                {
                    MaxTokens = 256,
                    Temperature = 0.3,
                    Classes = new[] { "positive", "negative", "neutral" },
                    ConfidenceThreshold = 0.75
                }),
                ModelPath = "/models/sentiment_analysis_v1.8.pkl",
                IsActive = true
            },
            new AIModel
            {
                Name = "Response Generation",
                Description = "Generates contextual responses for chat conversations",
                Version = "v3.0",
                Type = AIModelType.ResponseGeneration,
                Status = AIModelStatus.Training,
                Accuracy = 89.3,
                DatasetSize = 8500,
                LastTrained = DateTime.UtcNow.AddDays(-3),
                LastUsed = DateTime.UtcNow.AddHours(-1),
                Configuration = JsonSerializer.Serialize(new
                {
                    MaxTokens = 1024,
                    Temperature = 0.8,
                    TopP = 0.95,
                    MaxLength = 500,
                    MinLength = 10
                }),
                ModelPath = "/models/response_generation_v3.0.pkl",
                IsActive = false
            },
            new AIModel
            {
                Name = "Content Moderation",
                Description = "Detects inappropriate content and spam in user messages",
                Version = "v1.5",
                Type = AIModelType.ContentModeration,
                Status = AIModelStatus.Active,
                Accuracy = 96.8,
                DatasetSize = 20000,
                LastTrained = DateTime.UtcNow.AddDays(-10),
                LastUsed = DateTime.UtcNow.AddMinutes(-5),
                Configuration = JsonSerializer.Serialize(new
                {
                    MaxTokens = 512,
                    Temperature = 0.1,
                    Classes = new[] { "safe", "spam", "inappropriate", "toxic" },
                    StrictMode = true
                }),
                ModelPath = "/models/content_moderation_v1.5.pkl",
                IsActive = true
            }
        };

        // Audit the entities
        foreach (var model in models)
        {
            model.Audit("System");
        }

        await context.AIModels.AddRangeAsync(models);
        await context.SaveChangesAsync();

        // Seed training jobs
        await SeedTrainingJobs(context, models);
        
        // Seed training history
        await SeedTrainingHistory(context, models);
    }

    private static async Task SeedTrainingJobs(ApplicationDbContext context, List<AIModel> models)
    {
        var jobs = new List<TrainingJob>();

        // Active training job for Response Generation
        var responseModel = models.First(m => m.Name == "Response Generation");
        var job1 = new TrainingJob
        {
            AIModelId = responseModel.Id,
            JobName = "Response Generation v3.0 Training",
            Status = TrainingJobStatus.InProgress,
            StartedAt = DateTime.UtcNow.AddHours(-2),
            EstimatedDuration = TimeSpan.FromHours(3),
            Parameters = JsonSerializer.Serialize(new
            {
                LearningRate = 0.001,
                BatchSize = 32,
                Epochs = 100,
                ValidationSplit = 0.2
            }),
            Priority = 1
        };
        job1.Audit("System");
        jobs.Add(job1);

        // Queued job for Intent Classification
        var intentModel = models.First(m => m.Name == "Intent Classification");
        var job2 = new TrainingJob
        {
            AIModelId = intentModel.Id,
            JobName = "Intent Classification Retraining",
            Status = TrainingJobStatus.Queued,
            StartedAt = DateTime.UtcNow.AddMinutes(30),
            EstimatedDuration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(15)),
            Parameters = JsonSerializer.Serialize(new
            {
                LearningRate = 0.0005,
                BatchSize = 64,
                Epochs = 80,
                ValidationSplit = 0.15
            }),
            Priority = 2
        };
        job2.Audit("System");
        jobs.Add(job2);

        await context.TrainingJobs.AddRangeAsync(jobs);
        await context.SaveChangesAsync();
    }

    private static async Task SeedTrainingHistory(ApplicationDbContext context, List<AIModel> models)
    {
        var histories = new List<TrainingHistory>();

        // Response Generation history
        var responseModel = models.First(m => m.Name == "Response Generation");
        var history1 = new TrainingHistory
        {
            AIModelId = responseModel.Id,
            Version = "v2.9",
            TrainingDate = DateTime.UtcNow.AddDays(-1),
            Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(45)),
            InitialAccuracy = 87.1,
            FinalAccuracy = 89.3,
            Improvement = 2.2,
            Result = TrainingResult.Success,
            TrainingLog = string.Join("\n", new[]
            {
                "Epoch 1/100 - Loss: 0.8234 - Accuracy: 0.7123 - Val_Loss: 0.7891 - Val_Accuracy: 0.7456",
                "Epoch 2/100 - Loss: 0.6891 - Accuracy: 0.7856 - Val_Loss: 0.6234 - Val_Accuracy: 0.8012",
                "Epoch 3/100 - Loss: 0.5234 - Accuracy: 0.8234 - Val_Loss: 0.5678 - Val_Accuracy: 0.8345",
                "...",
                "Epoch 98/100 - Loss: 0.1234 - Accuracy: 0.9412 - Val_Loss: 0.1456 - Val_Accuracy: 0.9234",
                "Epoch 99/100 - Loss: 0.1198 - Accuracy: 0.9418 - Val_Loss: 0.1423 - Val_Accuracy: 0.9267",
                "Epoch 100/100 - Loss: 0.1187 - Accuracy: 0.9420 - Val_Loss: 0.1401 - Val_Accuracy: 0.9289",
                "Training completed successfully!",
                "Model saved to /models/response_generation_v2.9.pkl"
            }),
            Metrics = JsonSerializer.Serialize(new
            {
                Precision = 0.921,
                Recall = 0.897,
                F1Score = 0.909,
                TrainingLoss = 0.1187,
                ValidationLoss = 0.1401,
                TrainingTime = "2h 45m"
            }),
            Epochs = 100,
            BatchSize = 32,
            LearningRate = 0.001,
            Notes = "Improved response quality with new training data"
        };
        history1.Audit("System");
        histories.Add(history1);

        // Intent Classification history
        var intentModel = models.First(m => m.Name == "Intent Classification");
        var history2 = new TrainingHistory
        {
            AIModelId = intentModel.Id,
            Version = "v2.0",
            TrainingDate = DateTime.UtcNow.AddDays(-3),
            Duration = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)),
            InitialAccuracy = 92.4,
            FinalAccuracy = 94.2,
            Improvement = 1.8,
            Result = TrainingResult.Success,
            TrainingLog = string.Join("\n", new[]
            {
                "Epoch 1/80 - Loss: 0.4567 - Accuracy: 0.8234 - Val_Loss: 0.4123 - Val_Accuracy: 0.8456",
                "Epoch 2/80 - Loss: 0.3891 - Accuracy: 0.8567 - Val_Loss: 0.3678 - Val_Accuracy: 0.8789",
                "...",
                "Epoch 79/80 - Loss: 0.0987 - Accuracy: 0.9567 - Val_Loss: 0.1123 - Val_Accuracy: 0.9345",
                "Epoch 80/80 - Loss: 0.0945 - Accuracy: 0.9578 - Val_Loss: 0.1089 - Val_Accuracy: 0.9367",
                "Training completed successfully!"
            }),
            Metrics = JsonSerializer.Serialize(new
            {
                Precision = 0.945,
                Recall = 0.938,
                F1Score = 0.941,
                TrainingLoss = 0.0945,
                ValidationLoss = 0.1089
            }),
            Epochs = 80,
            BatchSize = 64,
            LearningRate = 0.0005,
            Notes = "Enhanced intent recognition accuracy"
        };
        history2.Audit("System");
        histories.Add(history2);

        await context.TrainingHistories.AddRangeAsync(histories);
        await context.SaveChangesAsync();
    }
}
