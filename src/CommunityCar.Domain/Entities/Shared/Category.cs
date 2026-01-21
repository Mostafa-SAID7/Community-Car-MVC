using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Shared;

public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string? Description { get; private set; }
    public Guid? ParentCategoryId { get; private set; }

    public Category(string name, string slug, string? description = null, Guid? parentCategoryId = null)
    {
        Name = name;
        Slug = slug;
        Description = description;
        ParentCategoryId = parentCategoryId;
    }

    private Category() { }
}
