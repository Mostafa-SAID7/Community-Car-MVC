using System;

namespace CommunityCar.Application.Features.AI.ViewModels;

public class RecentTrainingViewModel
{
    public string Model { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Result { get; set; } = string.Empty;
    public string Improvement { get; set; } = string.Empty;
}