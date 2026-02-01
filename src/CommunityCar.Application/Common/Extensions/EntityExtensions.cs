using CommunityCar.Domain.Base;
using System.Reflection;

namespace CommunityCar.Application.Common.Extensions;

/// <summary>
/// Extensions for domain entities
/// </summary>
public static class EntityExtensions
{
    /// <summary>
    /// Checks if entity has been modified by comparing with original values
    /// </summary>
    public static bool HasChanges<T>(this T entity, T original) where T : class
    {
        if (entity == null || original == null)
            return entity != original;

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            var currentValue = property.GetValue(entity);
            var originalValue = property.GetValue(original);

            if (!Equals(currentValue, originalValue))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the changed properties between two entities
    /// </summary>
    public static Dictionary<string, (object? OldValue, object? NewValue)> GetChanges<T>(
        this T entity, T original) where T : class
    {
        var changes = new Dictionary<string, (object?, object?)>();

        if (entity == null || original == null)
            return changes;

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            var currentValue = property.GetValue(entity);
            var originalValue = property.GetValue(original);

            if (!Equals(currentValue, originalValue))
            {
                changes[property.Name] = (originalValue, currentValue);
            }
        }

        return changes;
    }

    /// <summary>
    /// Copies properties from source to target entity
    /// </summary>
    public static T CopyFrom<T>(this T target, T source, params string[] excludeProperties) where T : class
    {
        if (source == null || target == null)
            return target;

        var excludeSet = new HashSet<string>(excludeProperties, StringComparer.OrdinalIgnoreCase);
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && !excludeSet.Contains(p.Name));

        foreach (var property in properties)
        {
            var value = property.GetValue(source);
            property.SetValue(target, value);
        }

        return target;
    }

    /// <summary>
    /// Creates a shallow copy of the entity
    /// </summary>
    public static T ShallowCopy<T>(this T entity) where T : class, new()
    {
        if (entity == null)
            return new T();

        var copy = new T();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite);

        foreach (var property in properties)
        {
            var value = property.GetValue(entity);
            property.SetValue(copy, value);
        }

        return copy;
    }

    /// <summary>
    /// Checks if entity is soft deleted
    /// </summary>
    public static bool IsDeleted<T>(this T entity) where T : class
    {
        var isDeletedProperty = typeof(T).GetProperty("IsDeleted");
        if (isDeletedProperty?.PropertyType == typeof(bool))
        {
            return (bool)(isDeletedProperty.GetValue(entity) ?? false);
        }

        var deletedAtProperty = typeof(T).GetProperty("DeletedAt");
        if (deletedAtProperty?.PropertyType == typeof(DateTime?))
        {
            return ((DateTime?)deletedAtProperty.GetValue(entity)).HasValue;
        }

        return false;
    }

    /// <summary>
    /// Marks entity as deleted (soft delete)
    /// </summary>
    public static T MarkAsDeleted<T>(this T entity) where T : class
    {
        var isDeletedProperty = typeof(T).GetProperty("IsDeleted");
        if (isDeletedProperty?.PropertyType == typeof(bool) && isDeletedProperty.CanWrite)
        {
            isDeletedProperty.SetValue(entity, true);
        }

        var deletedAtProperty = typeof(T).GetProperty("DeletedAt");
        if (deletedAtProperty?.PropertyType == typeof(DateTime?) && deletedAtProperty.CanWrite)
        {
            deletedAtProperty.SetValue(entity, DateTime.UtcNow);
        }

        return entity;
    }

    /// <summary>
    /// Restores a soft deleted entity
    /// </summary>
    public static T Restore<T>(this T entity) where T : class
    {
        var isDeletedProperty = typeof(T).GetProperty("IsDeleted");
        if (isDeletedProperty?.PropertyType == typeof(bool) && isDeletedProperty.CanWrite)
        {
            isDeletedProperty.SetValue(entity, false);
        }

        var deletedAtProperty = typeof(T).GetProperty("DeletedAt");
        if (deletedAtProperty?.PropertyType == typeof(DateTime?) && deletedAtProperty.CanWrite)
        {
            deletedAtProperty.SetValue(entity, null);
        }

        return entity;
    }

    /// <summary>
    /// Updates audit fields for entity modification
    /// </summary>
    public static T UpdateAuditFields<T>(this T entity, Guid? userId = null) where T : class
    {
        var updatedAtProperty = typeof(T).GetProperty("UpdatedAt");
        if (updatedAtProperty?.PropertyType == typeof(DateTime) && updatedAtProperty.CanWrite)
        {
            updatedAtProperty.SetValue(entity, DateTime.UtcNow);
        }
        else if (updatedAtProperty?.PropertyType == typeof(DateTime?) && updatedAtProperty.CanWrite)
        {
            updatedAtProperty.SetValue(entity, DateTime.UtcNow);
        }

        if (userId.HasValue)
        {
            var updatedByProperty = typeof(T).GetProperty("UpdatedBy");
            if (updatedByProperty?.PropertyType == typeof(Guid) && updatedByProperty.CanWrite)
            {
                updatedByProperty.SetValue(entity, userId.Value);
            }
            else if (updatedByProperty?.PropertyType == typeof(Guid?) && updatedByProperty.CanWrite)
            {
                updatedByProperty.SetValue(entity, userId.Value);
            }
        }

        return entity;
    }

    /// <summary>
    /// Sets creation audit fields
    /// </summary>
    public static T SetCreationAuditFields<T>(this T entity, Guid? userId = null) where T : class
    {
        var createdAtProperty = typeof(T).GetProperty("CreatedAt");
        if (createdAtProperty?.PropertyType == typeof(DateTime) && createdAtProperty.CanWrite)
        {
            createdAtProperty.SetValue(entity, DateTime.UtcNow);
        }

        if (userId.HasValue)
        {
            var createdByProperty = typeof(T).GetProperty("CreatedBy");
            if (createdByProperty?.PropertyType == typeof(Guid) && createdByProperty.CanWrite)
            {
                createdByProperty.SetValue(entity, userId.Value);
            }
        }

        return entity.UpdateAuditFields(userId);
    }

    /// <summary>
    /// Validates entity using data annotations
    /// </summary>
    public static IEnumerable<string> Validate<T>(this T entity) where T : class
    {
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(entity);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        
        System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            entity, context, results, validateAllProperties: true);

        return results.Select(r => r.ErrorMessage ?? "Validation error");
    }

    /// <summary>
    /// Checks if entity is valid using data annotations
    /// </summary>
    public static bool IsValid<T>(this T entity) where T : class
    {
        return !entity.Validate().Any();
    }
}