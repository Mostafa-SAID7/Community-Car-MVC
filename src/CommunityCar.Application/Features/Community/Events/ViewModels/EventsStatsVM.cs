namespace CommunityCar.Application.Features.Community.Events.ViewModels;

public class EventsStatsVM
{
    public int TotalEvents { get; set; }
    public int UpcomingEvents { get; set; }
    public int ActiveEvents { get; set; }
    public int TotalAttendees { get; set; }
    public List<EventSummaryVM> FeaturedEvents { get; set; } = new();
    public List<EventSummaryVM> UpcomingEventsList { get; set; } = new();
    public Dictionary<string, int> EventsByTag { get; set; } = new();
}