using CommunityCar.Application.Features.Events.DTOs;
using CommunityCar.Domain.Entities.Community.Events;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IEventsRepository
{
    Task<Event?> GetByIdAsync(Guid id);
    Task<IEnumerable<Event>> GetAllAsync();
    Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetByOrganizerAsync(Guid organizerId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Event> Items, int TotalCount)> SearchAsync(EventsSearchRequest request, CancellationToken cancellationToken = default);
    Task AddAsync(Event eventEntity);
    Task UpdateAsync(Event eventEntity);
    Task DeleteAsync(Event eventEntity);
}