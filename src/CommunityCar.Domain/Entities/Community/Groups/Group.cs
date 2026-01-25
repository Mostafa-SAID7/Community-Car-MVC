using System;
using System.Collections.Generic;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Domain.Entities.Community.Groups;

public class Group : AggregateRoot
{
    // Basic Information
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? NameAr { get; private set; }
    public string? DescriptionAr { get; private set; }
    public string? Category { get; private set; }
    public string? CategoryAr { get; private set; }
    public string? Rules { get; private set; }
    public string? RulesAr { get; private set; }
    
    // Privacy and Access
    public GroupPrivacy Privacy { get; private set; }
    public bool RequiresApproval { get; private set; }
    
    // Ownership and Verification
    public Guid OwnerId { get; private set; }
    public bool IsVerified { get; private set; }
    public bool IsOfficial { get; private set; }
    
    // Media
    public string? CoverImageUrl { get; private set; }
    public string? AvatarUrl { get; private set; }
    
    // Statistics
    public int MemberCount { get; private set; }
    public int PostCount { get; private set; }
    public DateTime? LastActivityAt { get; private set; }
    
    // Location (optional for local groups)
    public string? Location { get; private set; }
    public string? LocationAr { get; private set; }
    
    // Tags for discoverability
    private readonly List<string> _tags = new();
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    public Group(
        string name, 
        string description, 
        GroupPrivacy privacy, 
        Guid ownerId,
        string? category = null,
        string? rules = null,
        bool requiresApproval = false,
        string? location = null)
    {
        Name = name;
        Description = description;
        Privacy = privacy;
        OwnerId = ownerId;
        Category = category;
        Rules = rules;
        RequiresApproval = requiresApproval;
        Location = location;
        MemberCount = 1; // Creator is first member
        PostCount = 0;
        IsVerified = false;
        IsOfficial = false;
        LastActivityAt = DateTime.UtcNow;
    }

    private Group() { }

    // Methods for updating group properties
    public void UpdateBasicInfo(string name, string description, string? category = null)
    {
        Name = name;
        Description = description;
        Category = category;
        Audit(UpdatedBy ?? "System");
    }

    public void UpdateArabicContent(string? nameAr, string? descriptionAr, string? categoryAr = null, string? rulesAr = null, string? locationAr = null)
    {
        NameAr = nameAr;
        DescriptionAr = descriptionAr;
        CategoryAr = categoryAr;
        RulesAr = rulesAr;
        LocationAr = locationAr;
        Audit(UpdatedBy ?? "System");
    }

    public void UpdateRules(string rules)
    {
        Rules = rules;
        Audit(UpdatedBy ?? "System");
    }

    public void UpdatePrivacy(GroupPrivacy privacy, bool requiresApproval)
    {
        Privacy = privacy;
        RequiresApproval = requiresApproval;
        Audit(UpdatedBy ?? "System");
    }

    public void UpdateCoverImage(string? coverImageUrl)
    {
        CoverImageUrl = coverImageUrl;
        Audit(UpdatedBy ?? "System");
    }

    public void UpdateAvatar(string? avatarUrl)
    {
        AvatarUrl = avatarUrl;
        Audit(UpdatedBy ?? "System");
    }

    public void Verify()
    {
        IsVerified = true;
        Audit(UpdatedBy ?? "System");
    }

    public void MarkAsOfficial()
    {
        IsOfficial = true;
        IsVerified = true; // Official groups are always verified
        Audit(UpdatedBy ?? "System");
    }

    public void IncrementMemberCount()
    {
        MemberCount++;
        LastActivityAt = DateTime.UtcNow;
    }

    public void DecrementMemberCount()
    {
        if (MemberCount > 0)
            MemberCount--;
        LastActivityAt = DateTime.UtcNow;
    }

    public void IncrementPostCount()
    {
        PostCount++;
        LastActivityAt = DateTime.UtcNow;
    }

    public void AddTag(string tag)
    {
        if (!_tags.Contains(tag))
        {
            _tags.Add(tag);
            Audit(UpdatedBy ?? "System");
        }
    }

    public void RemoveTag(string tag)
    {
        if (_tags.Remove(tag))
        {
            Audit(UpdatedBy ?? "System");
        }
    }

    public void UpdateLocation(string? location)
    {
        Location = location;
        Audit(UpdatedBy ?? "System");
    }
}
