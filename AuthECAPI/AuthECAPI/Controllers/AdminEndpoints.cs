using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthECAPI.Core.Interfaces;
using AuthECAPI.Application.Models;

namespace AuthECAPI.Controllers;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/admin/users", CreateUser).RequireAuthorization("AdminOnly");
        return app;
    }

    [Authorize]
    internal static async Task<IResult> CreateUser(
        IAuthService authService,
        [FromBody] AdminCreateUserModel model)
    {
        var result = await authService.AdminCreateUserAsync(model);
        return result.Succeeded
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}
