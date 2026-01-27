using CommunityCar.Infrastructure.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace CommunityCar.Web.Extensions;

public static class LocalizationExtensions
{
    public static IServiceCollection AddAppLocalization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        // Temporarily use default localization instead of custom SQL localizer
        // This avoids the DI lifetime issue while we get the app running
        // services.AddScoped<IStringLocalizerFactory, SqlStringLocalizerFactory>();

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var localizationSettings = configuration.GetSection("Localization");
            var supportedCultures = localizationSettings.GetSection("SupportedCultures").Get<string[]>() ?? new[] { "en-US" };
            
            var cultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();

            options.DefaultRequestCulture = new RequestCulture(localizationSettings["DefaultRequestCulture"] ?? "en-US");
            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;

            // Optional: Configure how culture is detected
            options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
            options.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider());
            options.RequestCultureProviders.Insert(2, new AcceptLanguageHeaderRequestCultureProvider());
        });

        return services;
    }
}



