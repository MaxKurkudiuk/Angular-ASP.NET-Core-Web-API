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
        app.MapGet("/ApplyForMaternityLeave", ApplyForMaternityLeave);
        app.MapGet("/Under10sAndFemale", Under10sAndFemale);
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

    [Authorize(Policy = "FemalesOnly", Roles = nameof(Roles.Teacher))]
    private static string ApplyForMaternityLeave()
    {
        return "Applied for maternity leave.";
    }

    [Authorize(Policy = "Under10")]
    [Authorize(Policy = "FemalesOnly")]
    private static string Under10sAndFemale()
    {
        return "Under 10 and female.";
    }
}
