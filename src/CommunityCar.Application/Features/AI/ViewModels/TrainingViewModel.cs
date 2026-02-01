using System;

namespace CommunityCar.Application.Features.AI.ViewModels;

public class TrainingViewModel
{
    public TrainingModelViewModel[] Models { get; set; } = Array.Empty<TrainingModelViewModel>();
    public TrainingQueueItemViewModel[] TrainingQueue { get; set; } = Array.Empty<TrainingQueueItemViewModel>();
    public RecentTrainingViewModel[] RecentTraining { get; set; } = Array.Empty<RecentTrainingViewModel>();
    public string? Error { get; set; }
}