using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CommunityCar.Web.Helpers;

/// <summary>
/// Helper class for validation operations
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validates an email address format
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;

        try
        {
            var emailAttribute = new EmailAddressAttribute();
            return emailAttribute.IsValid(email);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates a phone number format
    /// </summary>
    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return false;

        // Remove all non-digit characters
        var digitsOnly = Regex.Replace(phoneNumber, @"[^\d]", "");
        
        // Check if it's between 10-15 digits (international standard)
        return digitsOnly.Length >= 10 && digitsOnly.Length <= 15;
    }

    /// <summary>
    /// Validates a URL format
    /// </summary>
    public static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Validates password strength
    /// </summary>
    public static ValidationResult ValidatePassword(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required");
            return new ValidationResult(false, errors);
        }

        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            errors.Add("Password must contain at least one uppercase letter");

        if (!Regex.IsMatch(password, @"[a-z]"))
            errors.Add("Password must contain at least one lowercase letter");

        if (!Regex.IsMatch(password, @"\d"))
            errors.Add("Password must contain at least one number");

        if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':;{}|<>]"))
            errors.Add("Password must contain at least one special character");

        return new ValidationResult(errors.Count == 0, errors);
    }

    /// <summary>
    /// Validates username format
    /// </summary>
    public static ValidationResult ValidateUsername(string username)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(username))
        {
            errors.Add("Username is required");
            return new ValidationResult(false, errors);
        }

        if (username.Length < 3)
            errors.Add("Username must be at least 3 characters long");

        if (username.Length > 20)
            errors.Add("Username cannot be longer than 20 characters");

        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_-]+$"))
            errors.Add("Username can only contain letters, numbers, underscores, and hyphens");

        if (username.StartsWith("_") || username.StartsWith("-"))
            errors.Add("Username cannot start with underscore or hyphen");

        return new ValidationResult(errors.Count == 0, errors);
    }

    /// <summary>
    /// Validates file upload
    /// </summary>
    public static ValidationResult ValidateFileUpload(IFormFile file, string[] allowedExtensions, long maxSizeBytes)
    {
        var errors = new List<string>();

        if (file == null || file.Length == 0)
        {
            errors.Add("File is required");
            return new ValidationResult(false, errors);
        }

        if (file.Length > maxSizeBytes)
        {
            var maxSizeMB = maxSizeBytes / (1024 * 1024);
            errors.Add($"File size cannot exceed {maxSizeMB} MB");
        }

        var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
        {
            errors.Add($"File type not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
        }

        return new ValidationResult(errors.Count == 0, errors);
    }

    /// <summary>
    /// Validates image file specifically
    /// </summary>
    public static ValidationResult ValidateImageFile(IFormFile file, long maxSizeBytes = 5 * 1024 * 1024)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        return ValidateFileUpload(file, allowedExtensions, maxSizeBytes);
    }

    /// <summary>
    /// Validates age based on birth date
    /// </summary>
    public static ValidationResult ValidateAge(DateTime birthDate, int minAge = 13, int maxAge = 120)
    {
        var errors = new List<string>();
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age))
            age--;

        if (age < minAge)
            errors.Add($"You must be at least {minAge} years old");

        if (age > maxAge)
            errors.Add($"Age cannot exceed {maxAge} years");

        return new ValidationResult(errors.Count == 0, errors);
    }

    /// <summary>
    /// Validates a slug format
    /// </summary>
    public static ValidationResult ValidateSlug(string slug)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(slug))
        {
            errors.Add("Slug is required");
            return new ValidationResult(false, errors);
        }

        if (slug.Length < 3)
            errors.Add("Slug must be at least 3 characters long");

        if (slug.Length > 50)
            errors.Add("Slug cannot be longer than 50 characters");

        if (!Regex.IsMatch(slug, @"^[a-z0-9-]+$"))
            errors.Add("Slug can only contain lowercase letters, numbers, and hyphens");

        if (slug.StartsWith("-") || slug.EndsWith("-"))
            errors.Add("Slug cannot start or end with a hyphen");

        if (slug.Contains("--"))
            errors.Add("Slug cannot contain consecutive hyphens");

        return new ValidationResult(errors.Count == 0, errors);
    }

    /// <summary>
    /// Validates ModelState and returns formatted errors
    /// </summary>
    public static Dictionary<string, string[]> GetModelStateErrors(ModelStateDictionary modelState)
    {
        return modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
            );
    }

    /// <summary>
    /// Validates a collection of objects
    /// </summary>
    public static ValidationResult ValidateCollection<T>(IEnumerable<T> items, Func<T, ValidationResult> validator)
    {
        var allErrors = new List<string>();
        var isValid = true;

        foreach (var item in items)
        {
            var result = validator(item);
            if (!result.IsValid)
            {
                isValid = false;
                allErrors.AddRange(result.Errors);
            }
        }

        return new ValidationResult(isValid, allErrors);
    }

    /// <summary>
    /// Validates required fields
    /// </summary>
    public static ValidationResult ValidateRequired(params (string FieldName, object? Value)[] fields)
    {
        var errors = new List<string>();

        foreach (var (fieldName, value) in fields)
        {
            if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
            {
                errors.Add($"{fieldName} is required");
            }
        }

        return new ValidationResult(errors.Count == 0, errors);
    }

    /// <summary>
    /// Validates string length
    /// </summary>
    public static ValidationResult ValidateStringLength(string value, string fieldName, int minLength = 0, int maxLength = int.MaxValue)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(value))
        {
            if (minLength > 0)
                errors.Add($"{fieldName} is required");
        }
        else
        {
            if (value.Length < minLength)
                errors.Add($"{fieldName} must be at least {minLength} characters long");

            if (value.Length > maxLength)
                errors.Add($"{fieldName} cannot be longer than {maxLength} characters");
        }

        return new ValidationResult(errors.Count == 0, errors);
    }

    /// <summary>
    /// Validates numeric range
    /// </summary>
    public static ValidationResult ValidateRange<T>(T value, string fieldName, T min, T max) where T : IComparable<T>
    {
        var errors = new List<string>();

        if (value.CompareTo(min) < 0)
            errors.Add($"{fieldName} must be at least {min}");

        if (value.CompareTo(max) > 0)
            errors.Add($"{fieldName} cannot exceed {max}");

        return new ValidationResult(errors.Count == 0, errors);
    }

    /// <summary>
    /// Sanitizes input to prevent XSS
    /// </summary>
    public static string SanitizeInput(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        // Remove potentially dangerous characters
        input = Regex.Replace(input, @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", "", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"javascript:", "", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"vbscript:", "", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"onload", "", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"onerror", "", RegexOptions.IgnoreCase);

        return input.Trim();
    }

    /// <summary>
    /// Validation result class
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; }
        public List<string> Errors { get; }

        public ValidationResult(bool isValid, List<string> errors)
        {
            IsValid = isValid;
            Errors = errors ?? new List<string>();
        }

        public ValidationResult(bool isValid, string error) : this(isValid, new List<string> { error })
        {
        }
    }
}