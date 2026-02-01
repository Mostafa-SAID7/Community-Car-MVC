using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CommunityCar.Web.Extensions;

/// <summary>
/// Extension methods for model operations and validation
/// </summary>
public static class ModelExtensions
{
    /// <summary>
    /// Gets all validation errors from ModelState as a dictionary
    /// </summary>
    public static Dictionary<string, string[]> GetValidationErrors(this ModelStateDictionary modelState)
    {
        return modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
            );
    }

    /// <summary>
    /// Gets all validation errors as a flat list of strings
    /// </summary>
    public static List<string> GetAllValidationErrors(this ModelStateDictionary modelState)
    {
        return modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value?.Errors.Select(e => e.ErrorMessage) ?? Enumerable.Empty<string>())
            .ToList();
    }

    /// <summary>
    /// Gets the first validation error message
    /// </summary>
    public static string? GetFirstValidationError(this ModelStateDictionary modelState)
    {
        return modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value?.Errors.Select(e => e.ErrorMessage) ?? Enumerable.Empty<string>())
            .FirstOrDefault();
    }

    /// <summary>
    /// Adds a model error for a specific property
    /// </summary>
    public static void AddModelError(this ModelStateDictionary modelState, string key, string errorMessage)
    {
        modelState.AddModelError(key, errorMessage);
    }

    /// <summary>
    /// Adds multiple model errors
    /// </summary>
    public static void AddModelErrors(this ModelStateDictionary modelState, Dictionary<string, string> errors)
    {
        foreach (var error in errors)
        {
            modelState.AddModelError(error.Key, error.Value);
        }
    }

    /// <summary>
    /// Checks if a specific property has validation errors
    /// </summary>
    public static bool HasError(this ModelStateDictionary modelState, string key)
    {
        return modelState.ContainsKey(key) && modelState[key]?.Errors.Count > 0;
    }

    /// <summary>
    /// Gets validation errors for a specific property
    /// </summary>
    public static string[] GetErrorsFor(this ModelStateDictionary modelState, string key)
    {
        if (!modelState.ContainsKey(key) || modelState[key]?.Errors.Count == 0)
            return Array.Empty<string>();

        return modelState[key]?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>();
    }

    /// <summary>
    /// Clears all validation errors
    /// </summary>
    public static void ClearErrors(this ModelStateDictionary modelState)
    {
        modelState.Clear();
    }

    /// <summary>
    /// Clears validation errors for a specific property
    /// </summary>
    public static void ClearErrorsFor(this ModelStateDictionary modelState, string key)
    {
        if (modelState.ContainsKey(key))
        {
            modelState[key]?.Errors.Clear();
        }
    }

    /// <summary>
    /// Copies properties from source object to target object
    /// </summary>
    public static TTarget MapTo<TTarget>(this object source) where TTarget : new()
    {
        var target = new TTarget();
        var sourceType = source.GetType();
        var targetType = typeof(TTarget);

        var sourceProperties = sourceType.GetProperties();
        var targetProperties = targetType.GetProperties();

        foreach (var sourceProperty in sourceProperties)
        {
            var targetProperty = targetProperties.FirstOrDefault(p => 
                p.Name == sourceProperty.Name && 
                p.PropertyType == sourceProperty.PropertyType &&
                p.CanWrite);

            if (targetProperty != null && sourceProperty.CanRead)
            {
                var value = sourceProperty.GetValue(source);
                targetProperty.SetValue(target, value);
            }
        }

        return target;
    }

    /// <summary>
    /// Updates target object properties from source object
    /// </summary>
    public static void UpdateFrom<TSource>(this object target, TSource source)
    {
        if (source == null) return;

        var sourceType = typeof(TSource);
        var targetType = target.GetType();

        var sourceProperties = sourceType.GetProperties();
        var targetProperties = targetType.GetProperties();

        foreach (var sourceProperty in sourceProperties)
        {
            var targetProperty = targetProperties.FirstOrDefault(p => 
                p.Name == sourceProperty.Name && 
                p.PropertyType == sourceProperty.PropertyType &&
                p.CanWrite);

            if (targetProperty != null && sourceProperty.CanRead)
            {
                var value = sourceProperty.GetValue(source);
                targetProperty.SetValue(target, value);
            }
        }
    }

    /// <summary>
    /// Converts an object to a dictionary of property names and values
    /// </summary>
    public static Dictionary<string, object?> ToDictionary(this object obj)
    {
        if (obj == null)
            return new Dictionary<string, object?>();

        return obj.GetType()
            .GetProperties()
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name, p => p.GetValue(obj));
    }

    /// <summary>
    /// Checks if an object has a property with the specified name
    /// </summary>
    public static bool HasProperty(this object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName) != null;
    }

    /// <summary>
    /// Gets the value of a property by name
    /// </summary>
    public static object? GetPropertyValue(this object obj, string propertyName)
    {
        var property = obj.GetType().GetProperty(propertyName);
        return property?.GetValue(obj);
    }

    /// <summary>
    /// Sets the value of a property by name
    /// </summary>
    public static void SetPropertyValue(this object obj, string propertyName, object? value)
    {
        var property = obj.GetType().GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
    }
}