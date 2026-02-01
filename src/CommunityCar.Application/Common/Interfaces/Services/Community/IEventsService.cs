using CommunityCar.Application.Features.Community.Events.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community;

public interface IEventsService
{
    Task<EventVM?> GetEventByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EventVM?> GetEventBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<EventsSearchVM> SearchEventsAsync(EventsSearchVM request, CancellationToken cancellationToken = default);
    Task<EventVM> CreateEventAsync(CreateEventVM request, CancellationToken cancellationToken = default);
    Task<EventVM> UpdateEventAsync(Guid id, UpdateEventVM request, CancellationToken cancellationToken = default);
    Task<bool> DeleteEventAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventVM>> GetUpcomingEventsAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventVM>> GetUserEventsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> JoinEventAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<bool> LeaveEventAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<EventsStatsVM> GetEventsStatsAsync(CancellationToken cancellationToken = default);
}


