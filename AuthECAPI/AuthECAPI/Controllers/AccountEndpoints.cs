using AuthECAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AuthECAPI.Controllers;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/UserProfile", GetUserProfile);
        return app;
    }

    [Authorize]
    internal static async Task<IResult> GetUserProfile(
        ClaimsPrincipal user,
        IAccountService accountService)
    {
        var userId = user.Claims.First(x => x.Type == "userID").Value;
        var profile = await accountService.GetUserProfileAsync(userId);
        return Results.Ok(profile);
    }
}
