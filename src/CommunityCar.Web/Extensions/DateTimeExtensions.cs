using System.Globalization;

namespace CommunityCar.Web.Extensions;

/// <summary>
/// Extension methods for DateTime operations commonly used in web applications
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Converts DateTime to a relative time string (e.g., "2 hours ago")
    /// </summary>
    public static string ToRelativeTime(this DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalSeconds < 60)
            return "just now";

        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes == 1 ? "" : "s")} ago";

        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours == 1 ? "" : "s")} ago";

        if (timeSpan.TotalDays < 7)
            return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays == 1 ? "" : "s")} ago";

        if (timeSpan.TotalDays < 30)
            return $"{(int)(timeSpan.TotalDays / 7)} week{((int)(timeSpan.TotalDays / 7) == 1 ? "" : "s")} ago";

        if (timeSpan.TotalDays < 365)
            return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) == 1 ? "" : "s")} ago";

        return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) == 1 ? "" : "s")} ago";
    }

    /// <summary>
    /// Converts DateTime to a friendly display format
    /// </summary>
    public static string ToFriendlyDate(this DateTime dateTime)
    {
        var today = DateTime.Today;
        var date = dateTime.Date;

        if (date == today)
            return "Today";

        if (date == today.AddDays(-1))
            return "Yesterday";

        if (date == today.AddDays(1))
            return "Tomorrow";

        if (date > today.AddDays(-7) && date < today)
            return dateTime.ToString("dddd"); // Day of week

        if (date.Year == today.Year)
            return dateTime.ToString("MMM dd"); // Month and day

        return dateTime.ToString("MMM dd, yyyy"); // Full date
    }

    /// <summary>
    /// Converts DateTime to a friendly display format with time
    /// </summary>
    public static string ToFriendlyDateTime(this DateTime dateTime)
    {
        var friendlyDate = dateTime.ToFriendlyDate();
        var time = dateTime.ToString("h:mm tt");

        if (friendlyDate == "Today" || friendlyDate == "Yesterday" || friendlyDate == "Tomorrow")
            return $"{friendlyDate} at {time}";

        return $"{friendlyDate} at {time}";
    }

    /// <summary>
    /// Checks if the date is today
    /// </summary>
    public static bool IsToday(this DateTime dateTime)
    {
        return dateTime.Date == DateTime.Today;
    }

    /// <summary>
    /// Checks if the date is yesterday
    /// </summary>
    public static bool IsYesterday(this DateTime dateTime)
    {
        return dateTime.Date == DateTime.Today.AddDays(-1);
    }

    /// <summary>
    /// Checks if the date is tomorrow
    /// </summary>
    public static bool IsTomorrow(this DateTime dateTime)
    {
        return dateTime.Date == DateTime.Today.AddDays(1);
    }

    /// <summary>
    /// Checks if the date is in the current week
    /// </summary>
    public static bool IsThisWeek(this DateTime dateTime)
    {
        var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(7);
        return dateTime.Date >= startOfWeek && dateTime.Date < endOfWeek;
    }

    /// <summary>
    /// Checks if the date is in the current month
    /// </summary>
    public static bool IsThisMonth(this DateTime dateTime)
    {
        var today = DateTime.Today;
        return dateTime.Year == today.Year && dateTime.Month == today.Month;
    }

    /// <summary>
    /// Checks if the date is in the current year
    /// </summary>
    public static bool IsThisYear(this DateTime dateTime)
    {
        return dateTime.Year == DateTime.Today.Year;
    }

    /// <summary>
    /// Gets the start of the day (00:00:00)
    /// </summary>
    public static DateTime StartOfDay(this DateTime dateTime)
    {
        return dateTime.Date;
    }

    /// <summary>
    /// Gets the end of the day (23:59:59.999)
    /// </summary>
    public static DateTime EndOfDay(this DateTime dateTime)
    {
        return dateTime.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the week (Sunday)
    /// </summary>
    public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Sunday)
    {
        var diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
        return dateTime.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// Gets the end of the week (Saturday)
    /// </summary>
    public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Sunday)
    {
        return dateTime.StartOfWeek(startOfWeek).AddDays(7).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the month
    /// </summary>
    public static DateTime StartOfMonth(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1);
    }

    /// <summary>
    /// Gets the end of the month
    /// </summary>
    public static DateTime EndOfMonth(this DateTime dateTime)
    {
        return dateTime.StartOfMonth().AddMonths(1).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the year
    /// </summary>
    public static DateTime StartOfYear(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, 1, 1);
    }

    /// <summary>
    /// Gets the end of the year
    /// </summary>
    public static DateTime EndOfYear(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, 12, 31, 23, 59, 59, 999);
    }

    /// <summary>
    /// Calculates age based on birth date
    /// </summary>
    public static int CalculateAge(this DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        
        if (birthDate.Date > today.AddYears(-age))
            age--;
            
        return age;
    }

    /// <summary>
    /// Formats DateTime for different cultures
    /// </summary>
    public static string ToLocalizedString(this DateTime dateTime, string? cultureName = null)
    {
        var culture = string.IsNullOrEmpty(cultureName) 
            ? CultureInfo.CurrentCulture 
            : new CultureInfo(cultureName);
            
        return dateTime.ToString(culture);
    }

    /// <summary>
    /// Converts to Unix timestamp
    /// </summary>
    public static long ToUnixTimestamp(this DateTime dateTime)
    {
        return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
    }

    /// <summary>
    /// Converts from Unix timestamp
    /// </summary>
    public static DateTime FromUnixTimestamp(this long unixTimestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
    }

    /// <summary>
    /// Checks if the date is a weekend
    /// </summary>
    public static bool IsWeekend(this DateTime dateTime)
    {
        return dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;
    }

    /// <summary>
    /// Checks if the date is a weekday
    /// </summary>
    public static bool IsWeekday(this DateTime dateTime)
    {
        return !dateTime.IsWeekend();
    }

    /// <summary>
    /// Gets the next occurrence of a specific day of the week
    /// </summary>
    public static DateTime NextDayOfWeek(this DateTime dateTime, DayOfWeek dayOfWeek)
    {
        var daysUntilTarget = ((int)dayOfWeek - (int)dateTime.DayOfWeek + 7) % 7;
        if (daysUntilTarget == 0)
            daysUntilTarget = 7; // Next week if it's the same day
            
        return dateTime.AddDays(daysUntilTarget);
    }

    /// <summary>
    /// Gets the previous occurrence of a specific day of the week
    /// </summary>
    public static DateTime PreviousDayOfWeek(this DateTime dateTime, DayOfWeek dayOfWeek)
    {
        var daysSinceTarget = ((int)dateTime.DayOfWeek - (int)dayOfWeek + 7) % 7;
        if (daysSinceTarget == 0)
            daysSinceTarget = 7; // Previous week if it's the same day
            
        return dateTime.AddDays(-daysSinceTarget);
    }
}