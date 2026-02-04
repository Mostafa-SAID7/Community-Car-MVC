using CommunityCar.Application.Features.Community.Events.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community.Events;

public interface IEventsService
{
    Task<object> GetEventsAsync();
    Task<EventsSearchVM> SearchEventsAsync(string? searchTerm = null, int page = 1, int pageSize = 20);
    Task<EventsStatsVM> GetEventsStatsAsync();
    Task<EventVM?> GetEventByIdAsync(Guid id);
    Task<EventVM?> GetEventBySlugAsync(string slug);
    Task<EventVM> CreateEventAsync(CreateEventVM model);
    Task<bool> UpdateEventAsync(Guid id, EditEventVM model);
    Task<bool> DeleteEventAsync(Guid id);
    Task<List<EventVM>> GetUserEventsAsync(Guid userId);
    Task<bool> JoinEventAsync(Guid eventId, Guid userId);
    Task<bool> LeaveEventAsync(Guid eventId, Guid userId);
    Task<bool> ShareEventAsync(Guid eventId, Guid userId, string platform);
    Task<List<EventVM>> GetUpcomingEventsAsync(int limit = 10);
}