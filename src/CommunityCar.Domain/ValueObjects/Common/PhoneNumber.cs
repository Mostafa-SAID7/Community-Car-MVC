using CommunityCar.Domain.Base;
using System.Text.RegularExpressions;

namespace CommunityCar.Domain.ValueObjects.Common;

/// <summary>
/// Value object representing a phone number
/// </summary>
public class PhoneNumber : ValueObject
{
    private static readonly Regex PhoneRegex = new(
        @"^\+?[1-9]\d{1,14}$", // E.164 format
        RegexOptions.Compiled);

    public string Value { get; private set; }
    public string CountryCode { get; private set; }
    public string Number { get; private set; }

    private PhoneNumber(string value, string countryCode, string number)
    {
        Value = value;
        CountryCode = countryCode;
        Number = number;
    }

    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        // Remove all non-digit characters except +
        var cleaned = Regex.Replace(phoneNumber, @"[^\d+]", "");

        if (!PhoneRegex.IsMatch(cleaned))
            throw new ArgumentException("Invalid phone number format", nameof(phoneNumber));

        // Extract country code and number
        string countryCode;
        string number;

        if (cleaned.StartsWith("+"))
        {
            // International format
            if (cleaned.Length < 4)
                throw new ArgumentException("Phone number too short", nameof(phoneNumber));

            countryCode = cleaned.Substring(1, Math.Min(3, cleaned.Length - 3));
            number = cleaned.Substring(countryCode.Length + 1);
        }
        else
        {
            // Assume domestic format (would need country context in real app)
            countryCode = "1"; // Default to US/Canada
            number = cleaned;
        }

        return new PhoneNumber(cleaned, countryCode, number);
    }

    public string ToInternationalFormat()
    {
        return $"+{CountryCode}{Number}";
    }

    public string ToDisplayFormat()
    {
        // Simple formatting - in practice would be more sophisticated
        if (CountryCode == "1" && Number.Length == 10)
        {
            return $"({Number.Substring(0, 3)}) {Number.Substring(3, 3)}-{Number.Substring(6)}";
        }

        return ToInternationalFormat();
    }

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;

    public override string ToString() => ToDisplayFormat();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}