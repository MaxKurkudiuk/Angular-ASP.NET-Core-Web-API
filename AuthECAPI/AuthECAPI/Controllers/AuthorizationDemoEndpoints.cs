using AuthECAPI.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace AuthECAPI.Controllers;

public static class AuthorizationDemoEndpoints
{
    public static IEndpointRouteBuilder MapAuthorizationDemoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/AdminOnly", AdminOnly);
        app.MapGet("/AdminOrTeacher", AdminOrTeacher);
        app.MapGet("/LibraryMembersOnly", LibraryMembersOnly);
        return app;
    }

    [Authorize(Roles = nameof(Roles.Admin))]
    private static string AdminOnly()
    {
        return "Admin Only";
    }

    [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.Teacher)}")]
    private static string AdminOrTeacher()
    {
        return "Admin or Teacher";
    }

    [Authorize(Policy = "HasLibraryID")]
    private static string LibraryMembersOnly()
    {
        return "Library Members Only";
    }
}
