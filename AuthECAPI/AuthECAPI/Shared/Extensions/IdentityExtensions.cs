using AuthECAPI.Core.Entities;
using AuthECAPI.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace AuthECAPI.Shared.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityHandlersAndStores(this IServiceCollection services)
    {
        // Service from Identity Core
        services.AddIdentityApiEndpoints<AppUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
        return services;
    }

    public static IServiceCollection ConfigureIdentityOptions(this IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.User.RequireUniqueEmail = true;
        });
        return services;
    }

    // Auth = Authentication + Authorization
    public static IServiceCollection AddIdentityAuth(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme =
            x.DefaultChallengeScheme =
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(y =>
        {
            y.SaveToken = false;
            y.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        config["AppSettings:JWTSeecret"]!)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy("HasLibraryID", policy => policy.RequireClaim("libraryID"));
            options.AddPolicy("FemalesOnly", policy => policy.RequireClaim(ClaimTypes.Gender, "Female"));
            options.AddPolicy("Under10", policy => policy.RequireAssertion(context =>
            {
                var ageClaim = context.User.Claims.FirstOrDefault(claim => claim.Type == "age");
                return ageClaim is not null && int.TryParse(ageClaim.Value, out var age) && age < 10;
            }));
        });
        return services;
    }

    public static WebApplication AddIdentityAuthMiddlewares(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }

    public static WebApplication AddSeedData(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            using (var scope = app.Services.CreateScope())
            {
                SeedData.Initialize(scope.ServiceProvider).Wait();
            }
        }
        return app;
    }
}
