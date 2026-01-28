using System;

namespace CommunityCar.Web.Models.AI;

public class TrainingModelViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public double Accuracy { get; set; }
    public DateTime LastTrained { get; set; }
    public string Status { get; set; } = string.Empty;
    public int DatasetSize { get; set; }
}