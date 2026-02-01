using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Events;
using CommunityCar.Application.Features.Events.DTOs;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IEventsRepository : IBaseRepository<Event>
{
    Task<Event?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Event> Items, int TotalCount)> SearchAsync(EventsSearchRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetByOrganizerAsync(Guid userId, CancellationToken cancellationToken = default);
}
