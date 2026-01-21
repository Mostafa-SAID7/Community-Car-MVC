using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Profile;

public class UserProfile : AggregateRoot
{
    public Guid UserId { get; private set; } // Foreign key to Auth User
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? Bio { get; private set; }
    public string? AvatarUrl { get; private set; }
    
    // Address Value Object defined inline or separate
    public string? City { get; private set; }
    public string? Country { get; private set; }

    public UserProfile(Guid userId, string firstName, string lastName)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
    }

    // EF Core constructor
    private UserProfile() { }

    public void UpdateInfo(string bio, string city, string country)
    {
        Bio = bio;
        City = city;
        Country = country;
        Audit(UpdatedBy);
    }
}
