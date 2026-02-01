using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Events.DTOs;
using CommunityCar.Application.Features.Events.ViewModels;
using CommunityCar.Domain.Entities.Community.Events;

namespace CommunityCar.Application.Services.Community;

public class EventsService : IEventsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public EventsService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<EventVM?> GetEventByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
        return eventEntity == null ? null : _mapper.Map<EventVM>(eventEntity);
    }

    public async Task<EventVM?> GetEventBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var eventEntity = await _unitOfWork.Events.GetBySlugAsync(slug, cancellationToken);
        return eventEntity == null ? null : _mapper.Map<EventVM>(eventEntity);
    }

    public async Task<EventsSearchResponse> SearchEventsAsync(EventsSearchRequest request, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _unitOfWork.Events.SearchAsync(request, cancellationToken);
        
        var summaryItems = items.Select(eventEntity => 
        {
            var summary = _mapper.Map<CommunityCar.Application.Features.Events.DTOs.EventSummaryVM>(eventEntity);
            
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

        return new EventsSearchResponse
        {
            Items = summaryItems,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<EventVM> CreateEventAsync(CreateEventRequest request, CancellationToken cancellationToken = default)
    {
        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        var eventEntity = new Event(
            request.Title,
            request.Description,
            request.StartTime,
            request.EndTime,
            request.Location,
            currentUserId);

        eventEntity.UpdateArabicContent(request.TitleAr, request.DescriptionAr, request.LocationDetailsAr);

        // Set additional properties
        if (!string.IsNullOrWhiteSpace(request.LocationDetails) || 
            request.Latitude.HasValue || request.Longitude.HasValue)
        {
            eventEntity.UpdateLocationDetails(request.LocationDetails, request.Latitude, request.Longitude);
        }

        if (request.MaxAttendees.HasValue || request.RequiresApproval)
        {
            eventEntity.UpdateAttendanceSettings(request.MaxAttendees, request.RequiresApproval);
        }

        if (request.TicketPrice.HasValue || !string.IsNullOrWhiteSpace(request.TicketInfo))
        {
            eventEntity.UpdatePricing(request.TicketPrice, request.TicketInfo);
        }

        eventEntity.UpdateVisibility(request.IsPublic);

        if (!string.IsNullOrWhiteSpace(request.ExternalUrl) || !string.IsNullOrWhiteSpace(request.ContactInfo))
        {
            eventEntity.UpdateContactInfo(request.ExternalUrl, request.ContactInfo);
        }

        if (request.Tags?.Any() == true)
        {
            foreach (var tag in request.Tags)
            {
                eventEntity.AddTag(tag);
            }
        }

        if (request.ImageUrls?.Any() == true)
        {
            foreach (var imageUrl in request.ImageUrls)
            {
                eventEntity.AddImage(imageUrl);
            }
        }

        await _unitOfWork.Events.AddAsync(eventEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EventVM>(eventEntity);
    }

    public async Task<EventVM> UpdateEventAsync(Guid id, UpdateEventRequest request, CancellationToken cancellationToken = default)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
        if (eventEntity == null) throw new ArgumentException("Event not found");

        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");
        
        // Check if user can edit (organizer or admin)
        if (eventEntity.OrganizerId != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only edit your own events");
        }

        eventEntity.UpdateBasicInfo(request.Title, request.Description, request.StartTime, request.EndTime, request.Location);
        eventEntity.UpdateLocationDetails(request.LocationDetails, request.Latitude, request.Longitude);
        eventEntity.UpdateAttendanceSettings(request.MaxAttendees, request.RequiresApproval);
        eventEntity.UpdatePricing(request.TicketPrice, request.TicketInfo);
        eventEntity.UpdateVisibility(request.IsPublic);
        eventEntity.UpdateContactInfo(request.ExternalUrl, request.ContactInfo);
        
        eventEntity.UpdateArabicContent(request.TitleAr, request.DescriptionAr, request.LocationDetailsAr);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EventVM>(eventEntity);
    }

    public async Task<bool> DeleteEventAsync(Guid id, CancellationToken cancellationToken = default)
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
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<EventVM>> GetUpcomingEventsAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        var events = await _unitOfWork.Events.GetUpcomingEventsAsync(count, cancellationToken);
        return _mapper.Map<IEnumerable<EventVM>>(events);
    }

    public async Task<IEnumerable<EventVM>> GetUserEventsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var events = await _unitOfWork.Events.GetByOrganizerAsync(userId, cancellationToken);
        return _mapper.Map<IEnumerable<EventVM>>(events);
    }

    public async Task<bool> JoinEventAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventEntity == null) return false;

        if (!eventEntity.HasAvailableSpots)
        {
            throw new InvalidOperationException("Event is full");
        }

        eventEntity.IncrementAttendeeCount();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> LeaveEventAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventEntity == null) return false;

        eventEntity.DecrementAttendeeCount();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<EventsStatsVM> GetEventsStatsAsync(CancellationToken cancellationToken = default)
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
            FeaturedEvents = _mapper.Map<List<CommunityCar.Application.Features.Events.DTOs.EventSummaryVM>>(upcomingEvents.OrderByDescending(e => e.AttendeeCount).Take(3)),
            UpcomingEventsList = _mapper.Map<List<CommunityCar.Application.Features.Events.DTOs.EventSummaryVM>>(upcomingEvents.OrderBy(e => e.StartTime).Take(5)),
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


