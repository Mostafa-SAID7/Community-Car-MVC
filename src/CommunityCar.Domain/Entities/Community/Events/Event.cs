using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Community.Events;

public class Event : AggregateRoot
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string Location { get; private set; }
    public Guid OrganizerId { get; private set; }

    public Event(string title, string description, DateTime startTime, DateTime endTime, string location, Guid organizerId)
    {
        Title = title;
        Description = description;
        StartTime = startTime;
        EndTime = endTime;
        Location = location;
        OrganizerId = organizerId;
    }

    private Event() { }
}
