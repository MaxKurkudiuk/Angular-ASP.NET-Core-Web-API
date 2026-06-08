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
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            // Map the visual UI page (e.g., /scalar/v1)
            app.MapScalarApiReference();
        }
        return app;
    }
}
