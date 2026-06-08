using AuthECAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthECAPI.Controllers;

public static class IdentityUserEndpoints
{
    public static IEndpointRouteBuilder MapIdentityUserEndpoints(
        this IEndpointRouteBuilder app,
        IConfiguration config,
        IWebHostEnvironment env)
    {
        app.MapPost("/api/signup", async (
            UserManager<AppUser> userManager,
            [FromBody] UserRegiastrationModel userRegiastrationModel
            ) => {
                AppUser user = new()
                {
                    UserName = userRegiastrationModel.Email,
                    Email = userRegiastrationModel.Email,
                    FullName = userRegiastrationModel.FullName
                };
                var result = await userManager.CreateAsync(
                    user,
                    userRegiastrationModel.Password);

                if (result.Succeeded)
                    return Results.Ok(result);
                return Results.BadRequest(result);
            });

        app.MapPost("/api/signin", async (
            UserManager<AppUser> userManager,
            [FromBody] LoginModel loginModel
            ) => {
                var user = await userManager.FindByEmailAsync(loginModel.Email);
                if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
                {
                    var signInKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            config["AppSettings:JWTSeecret"]!));
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
        );
        return app;
    }
}
