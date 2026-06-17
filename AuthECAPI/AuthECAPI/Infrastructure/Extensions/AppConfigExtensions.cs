using AuthECAPI.Application.Models;

namespace AuthECAPI.Infrastructure.Extensions;

public static class AppConfigExtensions
{
    public static WebApplication ConfigureCors(
        this WebApplication app,
        IConfiguration config)
    {
        // 1. Fetch the origins array from appsettings.json
        var allowedOrigins = config.GetSection("AllowedOrigins").Get<string[]>() ?? [];

        app.UseCors(options =>
            options.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader());
        return app;
    }

    public static IServiceCollection AddAppConfig(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<AppSettings>(config.GetSection("AppSettings"));
        return services;
    }
}
