using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Services.AI;
using CommunityCar.Application.Features.AI.ViewModels;

namespace CommunityCar.Web.Controllers.AiAgent;

[Route("AiAgent/Training")]
public class AITrainingController : Controller
{
    private readonly IAIManagementService _aiManagementService;
    private readonly ILogger<AITrainingController> _logger;

    public AITrainingController(
        IAIManagementService aiManagementService,
        ILogger<AITrainingController> logger)
    {
        _aiManagementService = aiManagementService;
        _logger = logger;
    }

    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var models = await _aiManagementService.GetAllModelsAsync();
            var trainingQueue = await _aiManagementService.GetTrainingQueueAsync();
            var recentTraining = await _aiManagementService.GetRecentTrainingHistoryAsync(10);

            var model = new
            {
                Models = models.Select(m => new {
                    Id = m.Id,
                    Name = m.Name,
                    Version = m.Version,
                    Accuracy = Math.Round(m.Accuracy, 2),
                    LastTrained = m.LastTrained,
                    Status = m.Status.ToString(),
                    DatasetSize = m.DatasetSize,
                    IsActive = m.IsActive,
                    Type = m.Type.ToString()
                }).ToArray(),
                TrainingQueue = trainingQueue.Select(q => new {
                    Id = q.Id,
                    JobName = q.JobName,
                    Model = q.AIModel?.Name ?? "Unknown Model",
                    Status = q.Status.ToString(),
                    Priority = q.Priority,
                    EstimatedTime = FormatTimeSpan(q.EstimatedDuration),
                    StartedAt = q.StartedAt,
                    Progress = CalculateProgress(q)
                }).ToArray(),
                RecentTraining = recentTraining.Select(h => new {
                    Id = h.Id,
                    Model = h.AIModel?.Name ?? "Unknown Model",
                    Date = h.TrainingDate,
                    Duration = FormatTimeSpan(h.Duration),
                    Result = h.Result.ToString(),
                    FinalAccuracy = Math.Round(h.FinalAccuracy, 2),
                    InitialAccuracy = Math.Round(h.InitialAccuracy, 2),
                    Improvement = Math.Round(h.Improvement, 1),
                    Epochs = h.Epochs
                }).ToArray(),
                Error = (string?)null
            };

            return View("~/Views/AiAgent/Training/Index.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading training data");
            
            var errorModel = new
            {
                Models = new object[0],
                TrainingQueue = new object[0],
                RecentTraining = new object[0],
                Error = "Unable to load training data"
            };
            
            return View("~/Views/AiAgent/Training/Index.cshtml", errorModel);
        }
    }

    [HttpPost]
    [Route("Start")]
    public async Task<IActionResult> StartTraining()
    {
        try
        {
            _logger.LogInformation("Starting AI training session");
            
            var models = await _aiManagementService.GetAllModelsAsync();
            var trainableModels = models.Where(m => m.IsActive && m.Status != Domain.Enums.AI.AIModelStatus.Training).ToList();
            
            if (!trainableModels.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "No trainable models found"
                });
            }
            
            var queuedModels = new List<object>();
            
            foreach (var model in trainableModels)
            {
                try
                {
                    var trainingParameters = new
                    {
                        LearningRate = 0.001,
                        BatchSize = 32,
                        Epochs = 100,
                        ValidationSplit = 0.2,
                        EarlyStopping = true,
                        EarlyStoppingPatience = 10
                    };
                    
                    var trainingJob = await _aiManagementService.StartTrainingAsync(model.Id, trainingParameters);
                    queuedModels.Add(new
                    {
                        ModelName = model.Name,
                        JobId = trainingJob.Id,
                        EstimatedTime = FormatTimeSpan(trainingJob.EstimatedDuration),
                        Priority = trainingJob.Priority
                    });
                    
                    _logger.LogInformation("Queued training job for model {ModelName} with ID {JobId}", model.Name, trainingJob.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to queue training job for model {ModelName}", model.Name);
                }
            }
            
            if (queuedModels.Any())
            {
                return Json(new
                {
                    success = true,
                    message = $"Training session started successfully! Queued {queuedModels.Count} model(s) for training.",
                    data = new
                    {
                        queuedModels = queuedModels.ToArray(),
                        totalQueued = queuedModels.Count,
                        estimatedTotalTime = CalculateTotalEstimatedTime(queuedModels)
                    }
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to queue any models for training"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting training session");
            return Json(new
            {
                success = false,
                message = "Failed to start training session",
                error = ex.Message
            });
        }
    }

    [HttpPost]
    [Route("Retrain")]
    public async Task<IActionResult> RetrainModel([FromBody] RetrainModelRequestVM request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return Json(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = errors
                });
            }

            _logger.LogInformation("Queuing model {ModelName} for retraining", request.ModelName);
            
            var models = await _aiManagementService.GetAllModelsAsync();
            var model = models.FirstOrDefault(m => m.Name.Equals(request.ModelName, StringComparison.OrdinalIgnoreCase));
            
            if (model == null)
            {
                return Json(new
                {
                    success = false,
                    message = $"Model '{request.ModelName}' not found"
                });
            }

            var trainingQueue = await _aiManagementService.GetTrainingQueueAsync();
            var isAlreadyQueued = trainingQueue.Any(t => t.AIModel?.Name.Equals(request.ModelName, StringComparison.OrdinalIgnoreCase) == true);
            
            if (isAlreadyQueued)
            {
                return Json(new
                {
                    success = false,
                    message = $"Model '{request.ModelName}' is already queued for training"
                });
            }

            var trainingParameters = new
            {
                ModelId = model.Id,
                ModelName = request.ModelName,
                Description = request.Description ?? $"Retraining of {request.ModelName} model",
                IncrementalTraining = request.IncrementalTraining,
                LearningRate = 0.001,
                BatchSize = 32,
                MaxEpochs = 100,
                EarlyStopping = true,
                ValidationSplit = 0.2,
                RequestedBy = User.Identity?.Name ?? "System",
                RequestedAt = DateTime.UtcNow
            };
            
            var trainingJob = await _aiManagementService.RetrainModelAsync(request.ModelName, trainingParameters);
            
            _logger.LogInformation("Successfully queued model {ModelName} for retraining with job ID {JobId}", 
                request.ModelName, trainingJob.Id);
            
            return Json(new
            {
                success = true,
                message = $"Model '{request.ModelName}' successfully queued for retraining",
                data = new
                {
                    jobId = trainingJob.Id,
                    modelName = request.ModelName,
                    estimatedTime = FormatTimeSpan(trainingJob.EstimatedDuration),
                    status = trainingJob.Status.ToString(),
                    queuePosition = trainingQueue.Count() + 1,
                    incrementalTraining = request.IncrementalTraining,
                    priority = trainingJob.Priority,
                    startedAt = trainingJob.StartedAt
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error queuing model {ModelName} for retraining", request.ModelName);
            return Json(new
            {
                success = false,
                message = $"Failed to queue '{request.ModelName}' for retraining",
                error = ex.Message
            });
        }
    }

    [HttpGet]
    [Route("Status/{jobId}")]
    public async Task<IActionResult> GetTrainingStatus(Guid jobId)
    {
        try
        {
            var trainingJob = await _aiManagementService.GetTrainingJobAsync(jobId);
            
            if (trainingJob == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Training job not found"
                });
            }

            var progress = CalculateProgress(trainingJob);
            var remainingTime = CalculateRemainingTime(trainingJob);

            return Json(new
            {
                success = true,
                data = new
                {
                    jobId = trainingJob.Id,
                    jobName = trainingJob.JobName,
                    modelName = trainingJob.AIModel?.Name ?? "Unknown",
                    status = trainingJob.Status.ToString(),
                    progress = progress,
                    startedAt = trainingJob.StartedAt,
                    estimatedDuration = FormatTimeSpan(trainingJob.EstimatedDuration),
                    remainingTime = remainingTime,
                    priority = trainingJob.Priority,
                    errorMessage = trainingJob.ErrorMessage
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting training status for job {JobId}", jobId);
            return Json(new
            {
                success = false,
                message = "Failed to get training status",
                error = ex.Message
            });
        }
    }

    [HttpPost]
    [Route("Cancel/{jobId}")]
    public async Task<IActionResult> CancelTraining(Guid jobId)
    {
        try
        {
            _logger.LogInformation("Cancelling training job {JobId}", jobId);

            var result = await _aiManagementService.CancelTrainingJobAsync(jobId);
            
            if (!result)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to cancel training job. Job may not exist or cannot be cancelled."
                });
            }

            _logger.LogInformation("Successfully cancelled training job {JobId}", jobId);

            return Json(new
            {
                success = true,
                message = "Training job cancelled successfully",
                data = new
                {
                    jobId = jobId,
                    cancelledAt = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling training job {JobId}", jobId);
            return Json(new
            {
                success = false,
                message = "Failed to cancel training job",
                error = ex.Message
            });
        }
    }

    [HttpGet]
    [Route("Details/{modelName}/{date}")]
    public async Task<IActionResult> GetTrainingDetails(string modelName, string date)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(modelName) || string.IsNullOrWhiteSpace(date))
            {
                return Json(new
                {
                    success = false,
                    message = "Model name and date are required"
                });
            }

            if (!DateTime.TryParse(date, out var trainingDate))
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid date format"
                });
            }

            _logger.LogInformation("Getting training details for {ModelName} on {Date}", modelName, date);
            
            var trainingHistory = await _aiManagementService.GetTrainingDetailsAsync(modelName, trainingDate);
            
            if (trainingHistory == null)
            {
                return Json(new
                {
                    success = false,
                    message = $"No training details found for '{modelName}' on {trainingDate:yyyy-MM-dd}"
                });
            }

            var models = await _aiManagementService.GetAllModelsAsync();
            var model = models.FirstOrDefault(m => m.Name.Equals(modelName, StringComparison.OrdinalIgnoreCase));

            var trainingDetails = new
            {
                ModelName = trainingHistory.AIModel?.Name ?? modelName,
                ModelId = trainingHistory.AIModel?.Id ?? Guid.Empty,
                TrainingDate = trainingHistory.TrainingDate,
                Duration = FormatTimeSpan(trainingHistory.Duration),
                Status = trainingHistory.Result.ToString(),
                
                // Performance Metrics
                FinalAccuracy = Math.Round(trainingHistory.FinalAccuracy, 4),
                InitialAccuracy = Math.Round(trainingHistory.InitialAccuracy, 4),
                Improvement = Math.Round(trainingHistory.Improvement, 2),
                BestAccuracy = Math.Round(trainingHistory.FinalAccuracy, 4),
                
                // Training Configuration
                Epochs = trainingHistory.Epochs,
                BatchSize = trainingHistory.BatchSize,
                LearningRate = trainingHistory.LearningRate,
                ValidationSplit = 0.2,
                
                // Additional Information
                TrainingTime = FormatTimeSpan(trainingHistory.Duration),
                DatasetSize = model?.DatasetSize ?? 0,
                
                // Logs and Notes
                TrainingLog = trainingHistory.TrainingLog?.Split('\n', StringSplitOptions.RemoveEmptyEntries) ?? new string[0],
                Notes = trainingHistory.Notes,
                
                // Model Information
                ModelVersion = model?.Version ?? "Unknown",
                ModelType = model?.Type.ToString() ?? "Unknown",
                IsActive = model?.IsActive ?? false
            };
            
            return Json(new
            {
                success = true,
                data = trainingDetails
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting training details for {ModelName} on {Date}", modelName, date);
            return Json(new
            {
                success = false,
                message = "Failed to load training details",
                error = ex.Message
            });
        }
    }

    [HttpGet]
    [Route("Report/{modelName}/{date}")]
    public async Task<IActionResult> DownloadTrainingReport(string modelName, string date)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(modelName) || string.IsNullOrWhiteSpace(date))
            {
                return Json(new
                {
                    success = false,
                    message = "Model name and date are required"
                });
            }

            if (!DateTime.TryParse(date, out var trainingDate))
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid date format"
                });
            }

            _logger.LogInformation("Generating training report for {ModelName} on {Date}", modelName, date);
            
            var trainingHistory = await _aiManagementService.GetTrainingDetailsAsync(modelName, trainingDate);
            
            if (trainingHistory == null)
            {
                return Json(new
                {
                    success = false,
                    message = $"No training data found for '{modelName}' on {trainingDate:yyyy-MM-dd}"
                });
            }

            var reportUrl = await _aiManagementService.GenerateTrainingReportAsync(modelName, trainingDate);
            
            if (string.IsNullOrEmpty(reportUrl))
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to generate training report"
                });
            }

            var reportMetadata = new
            {
                ModelName = modelName,
                TrainingDate = trainingDate,
                ReportGeneratedAt = DateTime.UtcNow,
                ReportType = "Training Summary",
                FileName = GenerateReportFileName(modelName, trainingDate),
                FileSize = "~2.5 MB",
                ExpiresAt = DateTime.UtcNow.AddHours(48),
                
                Sections = new[]
                {
                    "Executive Summary",
                    "Training Configuration",
                    "Performance Metrics",
                    "Training Progress Charts",
                    "Loss and Accuracy Graphs",
                    "Epoch-by-Epoch Details",
                    "Model Comparison",
                    "Recommendations"
                },
                
                TrainingSummary = new
                {
                    Duration = FormatTimeSpan(trainingHistory.Duration),
                    FinalAccuracy = Math.Round(trainingHistory.FinalAccuracy, 2),
                    Improvement = Math.Round(trainingHistory.Improvement, 2),
                    Status = trainingHistory.Result.ToString(),
                    Epochs = trainingHistory.Epochs
                }
            };
            
            _logger.LogInformation("Successfully generated training report for {ModelName}", modelName);
            
            return Json(new
            {
                success = true,
                message = "Training report generated successfully",
                data = new
                {
                    downloadUrl = reportUrl,
                    metadata = reportMetadata
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating training report for {ModelName} on {Date}", modelName, date);
            return Json(new
            {
                success = false,
                message = "Failed to generate training report",
                error = ex.Message
            });
        }
    }

    private static string FormatTimeSpan(TimeSpan? timeSpan)
    {
        if (!timeSpan.HasValue) return "Unknown";
        
        var ts = timeSpan.Value;
        if (ts.TotalHours >= 1)
            return $"{(int)ts.TotalHours}h {ts.Minutes}m";
        else if (ts.TotalMinutes >= 1)
            return $"{ts.Minutes}m {ts.Seconds}s";
        else
            return $"{ts.Seconds}s";
    }

    private static int CalculateProgress(Domain.Entities.AI.TrainingJob job)
    {
        if (job.Status == Domain.Enums.AI.TrainingJobStatus.Completed) return 100;
        if (job.Status == Domain.Enums.AI.TrainingJobStatus.Failed || job.Status == Domain.Enums.AI.TrainingJobStatus.Cancelled) return 0;
        if (job.Status == Domain.Enums.AI.TrainingJobStatus.Queued) return 0;
        
        // For in-progress jobs, estimate based on time elapsed
        if (job.Status == Domain.Enums.AI.TrainingJobStatus.InProgress && job.EstimatedDuration.HasValue)
        {
            var elapsed = DateTime.UtcNow - job.StartedAt;
            var progress = (int)((elapsed.TotalMinutes / job.EstimatedDuration.Value.TotalMinutes) * 100);
            return Math.Min(progress, 95); // Cap at 95% until actually completed
        }
        
        return 0;
    }

    private static string CalculateRemainingTime(Domain.Entities.AI.TrainingJob job)
    {
        if (job.Status != Domain.Enums.AI.TrainingJobStatus.InProgress || !job.EstimatedDuration.HasValue)
            return "Unknown";
        
        var elapsed = DateTime.UtcNow - job.StartedAt;
        var remaining = job.EstimatedDuration.Value - elapsed;
        
        if (remaining.TotalMinutes <= 0) return "Almost done";
        
        return FormatTimeSpan(remaining);
    }

    private static string CalculateTotalEstimatedTime(List<object> queuedModels)
    {
        // This would calculate based on actual estimated times from the queued models
        // For now, return a simple estimate
        var totalMinutes = queuedModels.Count * 45; // Assume 45 minutes per model on average
        return FormatTimeSpan(TimeSpan.FromMinutes(totalMinutes));
    }

    private static string GenerateReportFileName(string modelName, DateTime trainingDate)
    {
        var sanitizedName = modelName.ToLower().Replace(" ", "_").Replace("-", "_");
        var dateString = trainingDate.ToString("yyyyMMdd");
        return $"{sanitizedName}_training_report_{dateString}.pdf";
    }
}