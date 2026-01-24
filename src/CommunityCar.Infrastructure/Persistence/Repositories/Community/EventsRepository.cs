using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Features.Events.DTOs;
using CommunityCar.Domain.Entities.Community.Events;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class EventsRepository : IEventsRepository
{
    private readonly ApplicationDbContext _context;

    public EventsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Event?> GetByIdAsync(Guid id)
    {
        return await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _context.Events.ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Where(e => e.StartTime > DateTime.UtcNow && !e.IsCancelled)
            .OrderBy(e => e.StartTime)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetByOrganizerAsync(Guid organizerId, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Where(e => e.OrganizerId == organizerId)
            .OrderByDescending(e => e.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Event> Items, int TotalCount)> SearchAsync(EventsSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.Events.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(e => 
                e.Title.ToLower().Contains(searchTerm) ||
                e.Description.ToLower().Contains(searchTerm) ||
                e.Location.ToLower().Contains(searchTerm));
        }

        if (request.StartDate.HasValue)
        {
            query = query.Where(e => e.StartTime >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(e => e.EndTime <= request.EndDate.Value);
        }

        if (request.IsUpcomingOnly)
        {
            query = query.Where(e => e.StartTime > DateTime.UtcNow);
        }

        if (request.IsFreeOnly)
        {
            query = query.Where(e => !e.TicketPrice.HasValue || e.TicketPrice.Value == 0);
        }

        if (request.HasAvailableSpots)
        {
            query = query.Where(e => !e.MaxAttendees.HasValue || e.AttendeeCount < e.MaxAttendees.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            var location = request.Location.ToLower();
            query = query.Where(e => e.Location.ToLower().Contains(location));
        }

        if (request.Tags?.Any() == true)
        {
            foreach (var tag in request.Tags)
            {
                var tagLower = tag.ToLower();
                query = query.Where(e => e.Tags.Any(t => t.Contains(tagLower)));
            }
        }

        // Apply sorting
        query = request.SortBy?.ToLower() switch
        {
            "date" => request.SortDescending ? 
                query.OrderByDescending(e => e.StartTime) : 
                query.OrderBy(e => e.StartTime),
            "attendees" => request.SortDescending ? 
                query.OrderByDescending(e => e.AttendeeCount) : 
                query.OrderBy(e => e.AttendeeCount),
            "title" => request.SortDescending ? 
                query.OrderByDescending(e => e.Title) : 
                query.OrderBy(e => e.Title),
            _ => query.OrderBy(e => e.StartTime)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Event eventEntity)
    {
        await _context.Events.AddAsync(eventEntity);
    }

    public Task UpdateAsync(Event eventEntity)
    {
        _context.Events.Update(eventEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Event eventEntity)
    {
        _context.Events.Remove(eventEntity);
        return Task.CompletedTask;
    }
}