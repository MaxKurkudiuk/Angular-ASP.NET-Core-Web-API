using AuthECAPI.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace AuthECAPI.Controllers;

public static class AuthorizationDemoEndpoints
{
    public static IEndpointRouteBuilder MapAuthorizationDemoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/AdminOnly", AdminOnly);
        return app;
    }

    [Authorize(Roles = nameof(Roles.Admin))]
    private static string AdminOnly()
    {
        return "Admin Only";
    }
}
