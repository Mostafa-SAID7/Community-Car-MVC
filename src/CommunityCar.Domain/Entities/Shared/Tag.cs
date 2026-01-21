using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Shared;

public class Tag : BaseEntity
{
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public int UsageCount { get; private set; }

    public Tag(string name, string slug)
    {
        Name = name;
        Slug = slug;
        UsageCount = 0;
    }

    private Tag() { }

    public void IncrementUsage()
    {
        UsageCount++;
    }
}
