using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Features.Events.DTOs;
using CommunityCar.Domain.Entities.Community.Events;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class EventsRepository : BaseRepository<Event>, IEventsRepository
{
    public EventsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Event?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await Context.Events
            .FirstOrDefaultAsync(e => e.Slug == slug, cancellationToken);
    }

    public async Task<(IEnumerable<Event> Items, int TotalCount)> SearchAsync(EventsSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = Context.Events.AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
            query = query.Where(e => e.Title.Contains(request.SearchTerm));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await Context.Events
            .Where(e => e.StartTime > DateTime.UtcNow)
            .OrderBy(e => e.StartTime)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetByOrganizerAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Events
            .Where(e => e.OrganizerId == userId)
            .ToListAsync(cancellationToken);
    }
}
