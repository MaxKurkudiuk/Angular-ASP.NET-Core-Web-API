using AuthECAPI.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthECAPI.Shared.Extensions;

public static class EFCoreExtensions
{
    public static IServiceCollection InjectDbContext(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                config.GetConnectionString("DevDB"),
                sqlOptions => sqlOptions.EnableRetryOnFailure())
        );
        return services;
    }
}
