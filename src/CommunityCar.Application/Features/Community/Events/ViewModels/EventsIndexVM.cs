namespace CommunityCar.Application.Features.Community.Events.ViewModels;

public class EventsIndexVM
{
    public EventsSearchVM SearchRequest { get; set; } = new();
    public EventsSearchVM SearchResponse { get; set; } = new();
    public EventsStatsVM Stats { get; set; } = new();
}