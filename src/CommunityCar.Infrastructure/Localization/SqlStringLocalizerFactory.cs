using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace CommunityCar.Infrastructure.Localization;

public class SqlStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly ILocalizationService _localizationService;
    private readonly ResourceManagerStringLocalizerFactory _resourceManagerFactory;

    public SqlStringLocalizerFactory(
        ILocalizationService localizationService,
        IOptions<LocalizationOptions> localizationOptions)
    {
        _localizationService = localizationService;
        _resourceManagerFactory = new ResourceManagerStringLocalizerFactory(localizationOptions, new Microsoft.Extensions.Logging.LoggerFactory());
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        var fallbackLocalizer = _resourceManagerFactory.Create(resourceSource);
        return new SqlStringLocalizer(_localizationService, fallbackLocalizer, resourceSource.FullName);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        var fallbackLocalizer = _resourceManagerFactory.Create(baseName, location);
        return new SqlStringLocalizer(_localizationService, fallbackLocalizer, baseName);
    }
}

