using CommunityCar.Application.Features.Events.DTOs;
using CommunityCar.Application.Features.Events.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community;

public interface IEventsService
{
    Task<EventVM?> GetEventByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EventsSearchResponse> SearchEventsAsync(EventsSearchRequest request, CancellationToken cancellationToken = default);
    Task<EventVM> CreateEventAsync(CreateEventRequest request, CancellationToken cancellationToken = default);
    Task<EventVM> UpdateEventAsync(Guid id, UpdateEventRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteEventAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventVM>> GetUpcomingEventsAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventVM>> GetUserEventsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> JoinEventAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<bool> LeaveEventAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<EventsStatsVM> GetEventsStatsAsync(CancellationToken cancellationToken = default);
}