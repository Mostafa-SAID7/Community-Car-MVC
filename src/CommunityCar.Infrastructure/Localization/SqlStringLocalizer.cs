using CommunityCar.Application.Common.Interfaces.Services.Localization;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace CommunityCar.Infrastructure.Localization;

public class SqlStringLocalizer : IStringLocalizer
{
    private readonly ILocalizationService _localizationService;
    private readonly IStringLocalizer _fallbackLocalizer;
    private readonly string? _resourceGroup;

    public SqlStringLocalizer(
        ILocalizationService localizationService,
        IStringLocalizer fallbackLocalizer,
        string? resourceGroup = null)
    {
        _localizationService = localizationService;
        _fallbackLocalizer = fallbackLocalizer;
        _resourceGroup = resourceGroup;
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value ?? name, value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var format = GetString(name);
            var value = string.Format(format ?? name, arguments);
            return new LocalizedString(name, value, format == null);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _fallbackLocalizer.GetAllStrings(includeParentCultures);
    }

    private string? GetString(string name)
    {
        var culture = CultureInfo.CurrentUICulture.Name;
        
        // Sync version of getting resource
        // Since IStringLocalizer is synchronous, we have to block or use a cache
        // For simplicity in this implementation, we use Task.Run(() => ...).Result or similar
        // Better approach would be a memory cache
        var value = _localizationService.GetResourceValueAsync(name, culture, _resourceGroup).GetAwaiter().GetResult();

        if (string.IsNullOrEmpty(value))
        {
            return _fallbackLocalizer[name].Value;
        }

        return value;
    }
}
