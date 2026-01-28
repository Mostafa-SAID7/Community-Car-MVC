namespace CommunityCar.Web.Models.AI;

public class TrainingQueueItemViewModel
{
    public string Model { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string EstimatedTime { get; set; } = string.Empty;
}