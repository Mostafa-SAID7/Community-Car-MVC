using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Helpers;

/// <summary>
/// Helper class for managing ViewBag data consistently across controllers
/// </summary>
public static class ViewBagHelper
{
    /// <summary>
    /// Sets profile data in ViewBag with consistent property names
    /// </summary>
    public static void SetProfileData(dynamic viewBag, object profile)
    {
        if (profile == null) return;

        var profileType = profile.GetType();
        
        // Use reflection to set ViewBag properties from profile object
        foreach (var property in profileType.GetProperties())
        {
            var value = property.GetValue(profile);
            SetViewBagProperty(viewBag, property.Name, value);
        }
    }

    /// <summary>
    /// Sets user statistics in ViewBag
    /// </summary>
    public static void SetUserStats(dynamic viewBag, object? stats)
    {
        if (stats == null)
        {
            viewBag.Level = 1;
            viewBag.TotalPoints = 0;
            viewBag.Rank = "Beginner";
            viewBag.BadgesCount = 0;
            return;
        }

        var statsType = stats.GetType();
        foreach (var property in statsType.GetProperties())
        {
            var value = property.GetValue(stats);
            SetViewBagProperty(viewBag, property.Name, value);
        }
    }

    /// <summary>
    /// Sets pagination data in ViewBag
    /// </summary>
    public static void SetPaginationData(dynamic viewBag, int currentPage, int totalPages, int totalItems, int pageSize)
    {
        viewBag.CurrentPage = currentPage;
        viewBag.TotalPages = totalPages;
        viewBag.TotalItems = totalItems;
        viewBag.PageSize = pageSize;
        viewBag.HasPreviousPage = currentPage > 1;
        viewBag.HasNextPage = currentPage < totalPages;
        viewBag.StartItem = (currentPage - 1) * pageSize + 1;
        viewBag.EndItem = Math.Min(currentPage * pageSize, totalItems);
    }

    /// <summary>
    /// Sets breadcrumb data in ViewBag
    /// </summary>
    public static void SetBreadcrumbs(dynamic viewBag, params (string Text, string? Url)[] breadcrumbs)
    {
        viewBag.Breadcrumbs = breadcrumbs.Select(b => new { Text = b.Text, Url = b.Url }).ToList();
    }

    /// <summary>
    /// Sets page metadata for SEO
    /// </summary>
    public static void SetPageMetadata(dynamic viewBag, string title, string? description = null, string? keywords = null, string? image = null)
    {
        viewBag.PageTitle = title;
        viewBag.PageDescription = description;
        viewBag.PageKeywords = keywords;
        viewBag.PageImage = image;
        viewBag.PageUrl = null; // Will be set by controller if needed
    }

    /// <summary>
    /// Sets navigation state
    /// </summary>
    public static void SetNavigationState(dynamic viewBag, string activeSection, string? activeSubSection = null)
    {
        viewBag.ActiveSection = activeSection;
        viewBag.ActiveSubSection = activeSubSection;
    }

    /// <summary>
    /// Sets form data for edit scenarios
    /// </summary>
    public static void SetFormData(dynamic viewBag, bool isEdit, object? model = null)
    {
        viewBag.IsEdit = isEdit;
        viewBag.FormTitle = isEdit ? "Edit" : "Create";
        viewBag.SubmitButtonText = isEdit ? "Update" : "Create";
        
        if (model != null)
        {
            viewBag.FormModel = model;
        }
    }

    /// <summary>
    /// Sets error state data
    /// </summary>
    public static void SetErrorState(dynamic viewBag, string? errorMessage = null, string? errorCode = null)
    {
        viewBag.HasError = !string.IsNullOrEmpty(errorMessage);
        viewBag.ErrorMessage = errorMessage;
        viewBag.ErrorCode = errorCode;
    }

    /// <summary>
    /// Sets loading state data
    /// </summary>
    public static void SetLoadingState(dynamic viewBag, bool isLoading = false, string? loadingMessage = null)
    {
        viewBag.IsLoading = isLoading;
        viewBag.LoadingMessage = loadingMessage ?? "Loading...";
    }

    /// <summary>
    /// Sets culture-specific data
    /// </summary>
    public static void SetCultureData(dynamic viewBag, string culture, bool isRtl = false)
    {
        viewBag.Culture = culture;
        viewBag.IsRtl = isRtl;
        viewBag.Direction = isRtl ? "rtl" : "ltr";
    }

    /// <summary>
    /// Sets common dashboard data
    /// </summary>
    public static void SetDashboardData(dynamic viewBag, string section, object? stats = null)
    {
        viewBag.DashboardSection = section;
        if (stats != null)
        {
            SetUserStats(viewBag, stats);
        }
    }

    /// <summary>
    /// Helper method to safely set ViewBag properties
    /// </summary>
    private static void SetViewBagProperty(dynamic viewBag, string propertyName, object? value)
    {
        try
        {
            ((IDictionary<string, object?>)viewBag)[propertyName] = value;
        }
        catch
        {
            // Ignore errors when setting ViewBag properties
        }
    }

    /// <summary>
    /// Clears all ViewBag data
    /// </summary>
    public static void ClearViewBag(dynamic viewBag)
    {
        if (viewBag is IDictionary<string, object?> dict)
        {
            dict.Clear();
        }
    }

    /// <summary>
    /// Sets default values for common ViewBag properties
    /// </summary>
    public static void SetDefaults(dynamic viewBag)
    {
        viewBag.PageTitle = "Community Car";
        viewBag.PageDescription = "Community Car - Connect with car enthusiasts";
        viewBag.IsLoading = false;
        viewBag.HasError = false;
        viewBag.Culture = "en-US";
        viewBag.IsRtl = false;
        viewBag.Direction = "ltr";
    }
}