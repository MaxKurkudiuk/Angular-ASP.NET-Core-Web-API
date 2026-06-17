using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthECAPI.Application.Models;
using AuthECAPI.Core.Entities;
using AuthECAPI.Core.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthECAPI.Application.Services;

public class TokenService : ITokenService
{
    private readonly AppSettings _appSettings;
    private readonly IWebHostEnvironment _env;

    public TokenService(IOptions<AppSettings> appSettings, IWebHostEnvironment env)
    {
        _appSettings = appSettings.Value;
        _env = env;
    }

    public string GenerateToken(AppUser user, IList<string> roles)
    {
        var signInKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_appSettings.JWTSeecret));

        var claims = new List<Claim>
        {
            new("userID", user.Id.ToString()),
            new("gender", user.Gender),
            new("age", (DateTime.Now.Year - user.DOB.Year).ToString()),
            new(ClaimTypes.Role, roles.First())
        };

        if (user.LibraryID.HasValue)
            claims.Add(new Claim("libraryID", user.LibraryID.ToString()!));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _env.IsDevelopment()
                ? DateTime.UtcNow.AddDays(1)
                : DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }
}
