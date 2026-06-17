using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthECAPI.Shared.Models;
using AuthECAPI.Shared.Services;

namespace AuthECAPI.Controllers;

public static class IdentityUserEndpoints
{
    public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/signup", CreateUser);
        app.MapPost("/signin", SignIn);
        return app;
    }

    [AllowAnonymous]
    internal static async Task<IResult> CreateUser(
        IAuthService authService,
        [FromBody] UserRegistrationModel userRegiastrationModel)
    {
        var result = await authService.SignUpAsync(userRegiastrationModel);
        return result.Succeeded
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    [AllowAnonymous]
    internal static async Task<IResult> SignIn(
        IAuthService authService,
        [FromBody] LoginModel loginModel)
    {
        var (token, errorMessage) = await authService.SignInAsync(loginModel);
        return token is not null
            ? Results.Ok(new { token })
            : Results.BadRequest(new { message = errorMessage });
    }
}
