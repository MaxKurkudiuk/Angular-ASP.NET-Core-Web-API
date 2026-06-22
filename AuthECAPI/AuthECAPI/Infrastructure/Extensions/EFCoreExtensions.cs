using AuthECAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthECAPI.Infrastructure.Extensions;

public static class EFCoreExtensions
{
    public static IServiceCollection InjectDbContext(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                config.GetConnectionString("DevDB"),
                sqlOptions => sqlOptions.EnableRetryOnFailure()
                    .CommandTimeout(60) // for free AZURE DB connection
                )
        );
        return services;
    }
}
