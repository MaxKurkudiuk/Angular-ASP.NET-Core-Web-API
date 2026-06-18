using AuthECAPI.Application.Models;
using AuthECAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AuthECAPI.Controllers;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/UserProfile", GetUserProfile);
        app.MapPut("/UserProfile", UpdateUserProfile);
        app.MapDelete("/UserProfile", DeleteUserProfileAsync);
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

    [Authorize]
    internal static async Task<IResult> UpdateUserProfile(
        ClaimsPrincipal user,
        IAccountService accountService,
        UpdateUserProfileModel model)
    {
        var userId = user.Claims.First(x => x.Type == "userID").Value;
        var (profile, errorMessage) = await accountService.UpdateUserProfileAsync(userId, model);

        if (errorMessage is not null)
            return Results.BadRequest(new { Error = errorMessage });

        return Results.Ok(profile);
    }

    [Authorize]
    internal static async Task<IResult> DeleteUserProfileAsync(
        ClaimsPrincipal user,
        IAccountService accountService)
    {
        var userId = user.Claims.First(x => x.Type == "userID").Value;
        var (isSuccess, errorMessage) = await accountService.DeleteUserProfileAsync(userId);

        if (!isSuccess)
            return Results.BadRequest(new { Error = errorMessage });

        return Results.Ok(new { Message = "User profile deleted successfully." });
    }
}
