using AuthECAPI.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
    private static async Task<IResult> CreateUser(
        UserManager<AppUser> userManager,
        [FromBody] UserRegiastrationModel userRegiastrationModel)
    {
        AppUser user = new()
        {
            UserName = userRegiastrationModel.Email,
            Email = userRegiastrationModel.Email,
            FullName = userRegiastrationModel.FullName,
            Gender = userRegiastrationModel.Gender,
            // If today is 2024-01-15 and age is 25, result would be approximately 1999-01-15
            DOB = DateOnly.FromDateTime(DateTime.Now.AddYears(-userRegiastrationModel.Age)),
            LibraryID = userRegiastrationModel.LibraryID
        };
        var result = await userManager.CreateAsync(
            user,
            userRegiastrationModel.Password);
        await userManager.AddToRoleAsync(user, userRegiastrationModel.Role);

        if (result.Succeeded)
            return Results.Ok(result);
        return Results.BadRequest(result);
    }

    [AllowAnonymous]
    private static async Task<IResult> SignIn(
        UserManager<AppUser> userManager,
        [FromBody] LoginModel loginModel,
        IOptions<AppSettings> appSettingsOpt,
        IWebHostEnvironment env)
    {
        var user = await userManager.FindByEmailAsync(loginModel.Email);
        if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
        {
            var signInKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    appSettingsOpt.Value.JWTSeecret));
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserID", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
            };
            if (env.IsDevelopment())    // for testing
            {
                tokenDescriptor.Expires = DateTime.UtcNow.AddDays(1);
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return Results.Ok(new { token });
        }
        return Results.BadRequest(new { message = "Username or password is incorrect." });
    }
}
