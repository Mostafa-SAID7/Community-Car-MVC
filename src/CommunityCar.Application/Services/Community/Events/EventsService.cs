using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community.Events;
using CommunityCar.Application.Common.Interfaces.Services.Account.Core;
using CommunityCar.Application.Common.Interfaces.Services.Community.Broadcast;
using CommunityCar.Application.Features.Community.Events.ViewModels;
using CommunityCar.Domain.Entities.Community.Events;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Services.Community.Events;

public class EventsService : IEventsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBroadcastService _broadcastService;

    public EventsService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService, IBroadcastService broadcastService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _broadcastService = broadcastService;
    }

    public async Task<object> GetEventsAsync()
    {
        var upcomingEvents = await GetUpcomingEventsAsync(20);
        var stats = await GetEventsStatsAsync();
        
        return new
        {
            UpcomingEvents = upcomingEvents,
            Stats = stats
        };
    }

    public async Task<EventVM?> GetEventByIdAsync(Guid id)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
        return eventEntity == null ? null : _mapper.Map<EventVM>(eventEntity);
    }

    public async Task<EventVM?> GetEventBySlugAsync(string slug)
    {
        var eventEntity = await _unitOfWork.Events.GetBySlugAsync(slug, CancellationToken.None);
        return eventEntity == null ? null : _mapper.Map<EventVM>(eventEntity);
    }

    public async Task<EventsSearchVM> SearchEventsAsync(string? searchTerm = null, int page = 1, int pageSize = 20)
    {
        // Create search request from parameters
        var request = new EventsSearchVM
        {
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };

        var (items, totalCount) = await _unitOfWork.Events.SearchAsync(request, CancellationToken.None);
        
        var summaryItems = items.Select(eventEntity => 
        {
            var summary = _mapper.Map<CommunityCar.Application.Features.Community.Events.ViewModels.EventSummaryVM>(eventEntity);
            
            // Calculate distance if location provided
            if (request.Latitude.HasValue && request.Longitude.HasValue && 
                eventEntity.Latitude.HasValue && eventEntity.Longitude.HasValue)
            {
                summary.DistanceKm = CalculateDistance(
                    request.Latitude.Value, request.Longitude.Value,
                    eventEntity.Latitude.Value, eventEntity.Longitude.Value);
            }
            
            return summary;
        });

        return new EventsSearchVM
        {
            Items = summaryItems.ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<EventVM> CreateEventAsync(CreateEventVM request)
    {
        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        var eventEntity = new Event(
            request.Title,
            request.Description,
            request.StartDate,
            request.EndDate,
            request.Location,
            currentUserId);

        // Set additional properties - using mock values for missing properties
        eventEntity.UpdateArabicContent(null, null, null);

        // Set additional properties
        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            eventEntity.UpdateLocationDetails(null, null, null);
        }

        if (request.MaxAttendees > 0)
        {
            eventEntity.UpdateAttendanceSettings(request.MaxAttendees, false);
        }

        eventEntity.UpdatePricing(null, null);
        eventEntity.UpdateVisibility(request.IsPublic);
        eventEntity.UpdateContactInfo(null, null);

        if (request.Tags?.Any() == true)
        {
            foreach (var tag in request.Tags)
            {
                eventEntity.AddTag(tag);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            eventEntity.AddImage(request.ImageUrl);
        }

        await _unitOfWork.Events.AddAsync(eventEntity);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        return _mapper.Map<EventVM>(eventEntity);
    }

    public async Task<bool> UpdateEventAsync(Guid id, EditEventVM request)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
        if (eventEntity == null) return false;

        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");
        
        // Check if user can edit (organizer or admin)
        if (eventEntity.OrganizerId != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only edit your own events");
        }

        eventEntity.UpdateBasicInfo(request.Title, request.Description, request.StartDate, request.EndDate, request.Location);
        eventEntity.UpdateLocationDetails(null, null, null);
        eventEntity.UpdateAttendanceSettings(request.MaxAttendees, false);
        eventEntity.UpdatePricing(null, null);
        eventEntity.UpdateVisibility(request.IsPublic);
        eventEntity.UpdateContactInfo(null, null);
        
        eventEntity.UpdateArabicContent(null, null, null);

        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        return true;
    }

    public async Task<bool> DeleteEventAsync(Guid id)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
        if (eventEntity == null) return false;

        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");
        
        // Check if user can delete (organizer or admin)
        if (eventEntity.OrganizerId != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only delete your own events");
        }

        await _unitOfWork.Events.DeleteAsync(eventEntity);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        return true;
    }

    public async Task<List<EventVM>> GetUpcomingEventsAsync(int limit = 10)
    {
        var events = await _unitOfWork.Events.GetUpcomingEventsAsync(limit, CancellationToken.None);
        return _mapper.Map<List<EventVM>>(events);
    }

    public async Task<List<EventVM>> GetUserEventsAsync(Guid userId)
    {
        var events = await _unitOfWork.Events.GetByOrganizerAsync(userId, CancellationToken.None);
        return _mapper.Map<List<EventVM>>(events);
    }

    public async Task<bool> JoinEventAsync(Guid eventId, Guid userId)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventEntity == null) return false;

        if (!eventEntity.HasAvailableSpots)
        {
            throw new InvalidOperationException("Event is full");
        }

        eventEntity.IncrementAttendeeCount();
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        // Broadcast the join event
        await _broadcastService.BroadcastToUserAsync(userId.ToString(), $"You joined event: {eventEntity.Title}");

        return true;
    }

    public async Task<bool> LeaveEventAsync(Guid eventId, Guid userId)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventEntity == null) return false;

        eventEntity.DecrementAttendeeCount();
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        // Broadcast the leave event
        await _broadcastService.BroadcastToUserAsync(userId.ToString(), $"You left event: {eventEntity.Title}");

        return true;
    }

    public async Task<bool> ShareEventAsync(Guid eventId, Guid userId, string platform)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventEntity == null) return false;

        // Create share record
        var share = new Share(eventId, EntityType.Event, userId, ShareType.External, null, platform);
        await _unitOfWork.Shares.AddAsync(share);

        // Increment share count
        eventEntity.IncrementShareCount();
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        // Broadcast the share event
        await _broadcastService.BroadcastToAllAsync($"Event shared: {eventEntity.Title}");

        return true;
    }

    public async Task<EventsStatsVM> GetEventsStatsAsync()
    {
        var allEvents = await _unitOfWork.Events.GetAllAsync();
        var upcomingEvents = allEvents.Where(e => e.IsUpcoming).ToList();
        var activeEvents = allEvents.Where(e => e.IsActive).ToList();

        var stats = new EventsStatsVM
        {
            TotalEvents = allEvents.Count(),
            UpcomingEvents = upcomingEvents.Count,
            ActiveEvents = activeEvents.Count,
            TotalAttendees = allEvents.Sum(e => e.AttendeeCount),
            FeaturedEvents = _mapper.Map<List<CommunityCar.Application.Features.Community.Events.ViewModels.EventSummaryVM>>(upcomingEvents.OrderByDescending(e => e.AttendeeCount).Take(3)),
            UpcomingEventsList = _mapper.Map<List<CommunityCar.Application.Features.Community.Events.ViewModels.EventSummaryVM>>(upcomingEvents.OrderBy(e => e.StartTime).Take(5)),
            EventsByTag = allEvents
                .SelectMany(e => e.Tags)
                .GroupBy(tag => tag)
                .ToDictionary(g => g.Key, g => g.Count())
        };

        return stats;
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Haversine formula for calculating distance between two points on Earth
        const double R = 6371; // Earth's radius in kilometers

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}