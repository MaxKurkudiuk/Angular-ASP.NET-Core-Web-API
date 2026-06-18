using AuthECAPI.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        UserManager<AppUser> userManager)
    {
        var userId = user.Claims.First(x => x.Type == "userID").Value;
        var userDetails = await userManager.FindByIdAsync(userId);
        return Results.Ok(new
        {
            userDetails?.Email,
            userDetails?.FullName,
            userDetails?.Gender
        });
    }
}
