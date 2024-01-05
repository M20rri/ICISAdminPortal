using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace ICISAdminPortal.Infrastructure.Localization;
internal static class Startup
{
    internal static IServiceCollection AddPOLocalization(this IServiceCollection services, IConfiguration config)
    {
        services.AddLocalization(option => option.ResourcesPath = "Resources");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                    new CultureInfo("en"),
                    new CultureInfo("ar")
            };

            options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        return services;
    }
}