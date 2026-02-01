using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;

namespace CommunityCar.Web.Attributes;

/// <summary>
/// Sets culture for the current action
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class CultureAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Culture to set (e.g., "en-US", "ar-EG")
    /// </summary>
    public string Culture { get; set; }

    /// <summary>
    /// UI Culture to set (if different from Culture)
    /// </summary>
    public string? UICulture { get; set; }

    /// <summary>
    /// Whether to store culture in session
    /// </summary>
    public bool StoreInSession { get; set; } = true;

    /// <summary>
    /// Whether to store culture in cookie
    /// </summary>
    public bool StoreInCookie { get; set; } = true;

    /// <summary>
    /// Cookie expiration in days
    /// </summary>
    public int CookieExpirationDays { get; set; } = 30;

    public CultureAttribute(string culture)
    {
        Culture = culture ?? throw new ArgumentNullException(nameof(culture));
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        try
        {
            var cultureInfo = new CultureInfo(Culture);
            var uiCultureInfo = !string.IsNullOrEmpty(UICulture) ? new CultureInfo(UICulture) : cultureInfo;

            // Set current culture
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = uiCultureInfo;

            // Store in HttpContext for later use
            context.HttpContext.Items["Culture"] = Culture;
            context.HttpContext.Items["UICulture"] = UICulture ?? Culture;

            // Store in session if requested
            if (StoreInSession && context.HttpContext.Session != null)
            {
                context.HttpContext.Session.SetString("Culture", Culture);
                if (!string.IsNullOrEmpty(UICulture))
                {
                    context.HttpContext.Session.SetString("UICulture", UICulture);
                }
            }

            // Store in cookie if requested
            if (StoreInCookie)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(CookieExpirationDays),
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax
                };

                context.HttpContext.Response.Cookies.Append("Culture", Culture, cookieOptions);
                if (!string.IsNullOrEmpty(UICulture))
                {
                    context.HttpContext.Response.Cookies.Append("UICulture", UICulture, cookieOptions);
                }
            }
        }
        catch (CultureNotFoundException ex)
        {
            // Log the error and continue with default culture
            var logger = context.HttpContext.RequestServices.GetService<ILogger<CultureAttribute>>();
            logger?.LogWarning(ex, "Invalid culture specified: {Culture}", Culture);
        }

        base.OnActionExecuting(context);
    }
}

/// <summary>
/// Automatically detects and sets culture from various sources
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AutoCultureAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Supported cultures
    /// </summary>
    public string[] SupportedCultures { get; set; } = { "en-US", "ar-EG" };

    /// <summary>
    /// Default culture if none detected
    /// </summary>
    public string DefaultCulture { get; set; } = "en-US";

    /// <summary>
    /// Priority order for culture detection
    /// </summary>
    public CultureSource[] DetectionOrder { get; set; } = 
    {
        CultureSource.RouteData,
        CultureSource.QueryString,
        CultureSource.Cookie,
        CultureSource.Session,
        CultureSource.UserProfile,
        CultureSource.AcceptLanguage
    };

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var detectedCulture = DetectCulture(context);
        
        if (!string.IsNullOrEmpty(detectedCulture) && SupportedCultures.Contains(detectedCulture))
        {
            try
            {
                var cultureInfo = new CultureInfo(detectedCulture);
                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;

                context.HttpContext.Items["Culture"] = detectedCulture;
                context.HttpContext.Items["DetectedCulture"] = true;
            }
            catch (CultureNotFoundException)
            {
                SetDefaultCulture(context);
            }
        }
        else
        {
            SetDefaultCulture(context);
        }

        base.OnActionExecuting(context);
    }

    private string? DetectCulture(ActionExecutingContext context)
    {
        foreach (var source in DetectionOrder)
        {
            var culture = source switch
            {
                CultureSource.RouteData => GetCultureFromRoute(context),
                CultureSource.QueryString => GetCultureFromQuery(context),
                CultureSource.Cookie => GetCultureFromCookie(context),
                CultureSource.Session => GetCultureFromSession(context),
                CultureSource.UserProfile => GetCultureFromUserProfile(context),
                CultureSource.AcceptLanguage => GetCultureFromAcceptLanguage(context),
                _ => null
            };

            if (!string.IsNullOrEmpty(culture) && SupportedCultures.Contains(culture))
            {
                return culture;
            }
        }

        return null;
    }

    private string? GetCultureFromRoute(ActionExecutingContext context)
    {
        return context.RouteData.Values["culture"]?.ToString();
    }

    private string? GetCultureFromQuery(ActionExecutingContext context)
    {
        return context.HttpContext.Request.Query["culture"].FirstOrDefault();
    }

    private string? GetCultureFromCookie(ActionExecutingContext context)
    {
        return context.HttpContext.Request.Cookies["Culture"];
    }

    private string? GetCultureFromSession(ActionExecutingContext context)
    {
        return context.HttpContext.Session?.GetString("Culture");
    }

    private string? GetCultureFromUserProfile(ActionExecutingContext context)
    {
        // This would typically come from user profile/settings
        // Implementation depends on your user management system
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            return context.HttpContext.User.FindFirst("culture")?.Value;
        }
        return null;
    }

    private string? GetCultureFromAcceptLanguage(ActionExecutingContext context)
    {
        var acceptLanguage = context.HttpContext.Request.Headers.AcceptLanguage.FirstOrDefault();
        if (string.IsNullOrEmpty(acceptLanguage)) return null;

        var languages = acceptLanguage
            .Split(',')
            .Select(lang => lang.Split(';')[0].Trim())
            .Where(lang => !string.IsNullOrEmpty(lang));

        foreach (var language in languages)
        {
            // Try exact match first
            if (SupportedCultures.Contains(language))
            {
                return language;
            }

            // Try language part only (e.g., "en" from "en-US")
            var languagePart = language.Split('-')[0];
            var matchingCulture = SupportedCultures.FirstOrDefault(c => c.StartsWith(languagePart + "-"));
            if (!string.IsNullOrEmpty(matchingCulture))
            {
                return matchingCulture;
            }
        }

        return null;
    }

    private void SetDefaultCulture(ActionExecutingContext context)
    {
        try
        {
            var cultureInfo = new CultureInfo(DefaultCulture);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            context.HttpContext.Items["Culture"] = DefaultCulture;
            context.HttpContext.Items["DetectedCulture"] = false;
        }
        catch (CultureNotFoundException)
        {
            // Fallback to invariant culture
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
        }
    }
}

/// <summary>
/// Culture detection sources
/// </summary>
public enum CultureSource
{
    RouteData,
    QueryString,
    Cookie,
    Session,
    UserProfile,
    AcceptLanguage
}

/// <summary>
/// Forces Arabic culture
/// </summary>
public class ArabicCultureAttribute : CultureAttribute
{
    public ArabicCultureAttribute() : base("ar-EG")
    {
    }
}

/// <summary>
/// Forces English culture
/// </summary>
public class EnglishCultureAttribute : CultureAttribute
{
    public EnglishCultureAttribute() : base("en-US")
    {
    }
}