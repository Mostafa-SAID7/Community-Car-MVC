using System;

namespace CommunityCar.Web.Models.AI;

public class TrainingViewModel
{
    public TrainingModelViewModel[] Models { get; set; } = Array.Empty<TrainingModelViewModel>();
    public TrainingQueueItemViewModel[] TrainingQueue { get; set; } = Array.Empty<TrainingQueueItemViewModel>();
    public RecentTrainingViewModel[] RecentTraining { get; set; } = Array.Empty<RecentTrainingViewModel>();
    public string? Error { get; set; }
}