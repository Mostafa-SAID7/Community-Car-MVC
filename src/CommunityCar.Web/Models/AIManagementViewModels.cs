using System;

namespace CommunityCar.Web.Models
{
    public class TrainingViewModel
    {
        public TrainingModelViewModel[] Models { get; set; } = Array.Empty<TrainingModelViewModel>();
        public TrainingQueueItemViewModel[] TrainingQueue { get; set; } = Array.Empty<TrainingQueueItemViewModel>();
        public RecentTrainingViewModel[] RecentTraining { get; set; } = Array.Empty<RecentTrainingViewModel>();
        public string? Error { get; set; }
    }

    public class TrainingModelViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public double Accuracy { get; set; }
        public DateTime LastTrained { get; set; }
        public string Status { get; set; } = string.Empty;
        public int DatasetSize { get; set; }
    }

    public class TrainingQueueItemViewModel
    {
        public string Model { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string EstimatedTime { get; set; } = string.Empty;
    }

    public class RecentTrainingViewModel
    {
        public string Model { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Result { get; set; } = string.Empty;
        public string Improvement { get; set; } = string.Empty;
    }
}