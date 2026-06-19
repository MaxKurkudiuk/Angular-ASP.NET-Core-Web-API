using Scalar.AspNetCore;

namespace AuthECAPI.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiExplorer(this IServiceCollection services)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        return services;
    }

    public static WebApplication ConfigureOpenApiExplorer(this WebApplication app)
    {
        app.MapOpenApi().AllowAnonymous();
        app.MapScalarApiReference("/scalar").AllowAnonymous();
        return app;
    }
}
